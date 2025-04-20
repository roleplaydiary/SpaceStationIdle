using System;
using System.Linq;
using UniRx;
using UnityEngine;

public class UpgradeService : IDisposable
{
    private readonly StationController _stationController;
    private readonly ResourceManager _resourceManager;
    private readonly UpgradeDataSO _upgradeDataSO;
    private readonly ReactiveDictionary<string, bool> _purchasedUpgrades = new ReactiveDictionary<string, bool>();
    public IReadOnlyReactiveDictionary<string, bool> PurchasedUpgrades => _purchasedUpgrades;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public UpgradeService()
    {
        _stationController = ServiceLocator.Get<StationController>();
        _resourceManager = ServiceLocator.Get<ResourceManager>();
        _upgradeDataSO = ServiceLocator.Get<DataLibrary>().upgradeData;
    }
    
    public bool CanPurchaseUpgrade(string upgradeId)
    {
        UpgradeDataSO.UpgradeEntry upgrade = _upgradeDataSO.GetUpgradeById(upgradeId);
        if (upgrade.upgradeId == null) return false;

        PlayerController playerController = ServiceLocator.Get<PlayerController>();
        if (playerController == null || playerController.PlayerData == null)
        {
            Debug.LogError("PlayerController или PlayerData не найдены!");
            return false;
        }

        if (playerController.PlayerData.playerCredits.Value < upgrade.cost.credits) return false;
        if (playerController.PlayerData.researchPoints.Value < upgrade.cost.researchPoints) return false;

        if (upgrade.cost.resources != null)
        {
            Resources costResources = upgrade.cost.resources;

            foreach (var costResource in costResources)
            {
                string resourceName = costResource.Key;
                float requiredAmount = costResource.Value;
                float currentAmount = _resourceManager.GetResourceAmount(resourceName);

                if (currentAmount < requiredAmount)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void PurchaseUpgrade(string upgradeId, Department department = Department.Bridge)
    {
        var upgrade = _upgradeDataSO.GetUpgradeById(upgradeId);
        if (upgrade.upgradeId == null || (_purchasedUpgrades.ContainsKey(upgrade.upgradeId) && _purchasedUpgrades[upgrade.upgradeId])) return;
    
        if (CanPurchaseUpgrade(upgradeId))
        {
            // Списываем средства
            PlayerController playerController = ServiceLocator.Get<PlayerController>();
            playerController.AddCredits((int)-upgrade.cost.credits);
            playerController.PlayerData.researchPoints.Value -= upgrade.cost.researchPoints;
    
            // Списываем ресурсы
            if (upgrade.cost.resources != null)
            {
                foreach (var resourceCost in upgrade.cost.resources)
                {
                    _resourceManager.AddResource(resourceCost.Key, -resourceCost.Value);
                }
                
                _resourceManager.SaveResources(); // Сохраняем при изменении
            }
    
            // Применяем эффект апгрейда
            ApplyUpgrade(upgrade.type, department);
    
            // Помечаем апгрейд как купленный
            _purchasedUpgrades[upgrade.upgradeId] = true;

            _resourceManager.SaveResources();
        }
        else
        {
            Debug.LogWarning($"Невозможно купить апгрейд {upgradeId}. Недостаточно средств или ресурсов.");
            // Оповестить игрока о неудачной попытке покупки (через UI)
        }
    }
    
    public void ApplyUpgrade(UpgradeDataSO.UpgradeType type, Department department = Department.Bridge)
    {
        switch (type)
        {
            case UpgradeDataSO.UpgradeType.DepartmentUnlock:
                // Логика разблокировки департамента
                Debug.Log($"Запрос на разблокировку департамента: {department}");
                _stationController.UnlockStationBlock(department);
                break;
            case UpgradeDataSO.UpgradeType.DepartmentMaxCrew:
                // Логика увеличения максимального количества сотрудников в департаменте
                Debug.Log($"Запрос на увеличение макс. экипажа департамента: {department}");
                _stationController.UpgradeDepartmentMaxCrew(department);
                break;
            case UpgradeDataSO.UpgradeType.StationMaxCrew:
                // Логика увеличения максимального количества сотрудников на станции
                Debug.Log($"Запрос на увеличение макс. экипажа станции");
                StationController stationController = ServiceLocator.Get<StationController>();
                stationController.UpgradeStationMaxCrew();
                break;
            case UpgradeDataSO.UpgradeType.DepartmentCrewHire:
                // Логика найма дополнительного сотрудника в департамент
                Debug.Log($"Запрос на найм сотрудника в департамент: {department}");
                var departmentBlockForHire = _stationController.StationBlocks.FirstOrDefault(block => block.GetBlockType() == department);
                departmentBlockForHire?.HireNewCrewMember();
                break;
            case UpgradeDataSO.UpgradeType.DepartmentMaxWorkstations:
                // Логика увеличения максимального количества верстаков в департаменте
                Debug.Log($"Запрос на увеличение макс. верстаков департамента: {department}");
                var departmentBlockForMaxBenches = _stationController.StationBlocks.FirstOrDefault(block => block.GetBlockType() == department);
                departmentBlockForMaxBenches?.UpgradeWorkBenchMax();
                break;
            case UpgradeDataSO.UpgradeType.DepartmentWorkstationAdd:
                // Логика добавления нового верстака в департамент
                Debug.Log($"Запрос на добавление верстака в департамент: {department}");
                var departmentBlockForAddBench = _stationController.StationBlocks.FirstOrDefault(block => block.GetBlockType() == department);
                departmentBlockForAddBench?.AddWorkBench();
                break;
            default:
                Debug.LogWarning($"Неизвестный тип апгрейда: {type}");
                break;
        }
    }

    public void Dispose()
    {
        _disposables.Clear();
    }
}