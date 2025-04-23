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
    
    public bool CanPurchaseUpgrade(string upgradeId, Department department)
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
    if (!CheckUpgradeValue(department, upgrade)) return false;

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

    public async void PurchaseUpgrade(string upgradeId, Department department = Department.Bridge)
    {
        var upgrade = _upgradeDataSO.GetUpgradeById(upgradeId);
        if (upgrade.upgradeId == null || (_purchasedUpgrades.ContainsKey(upgrade.upgradeId) && _purchasedUpgrades[upgrade.upgradeId])) return;
    
        if (CanPurchaseUpgrade(upgradeId, department))
        {
            // Списываем средства
            PlayerController playerController = ServiceLocator.Get<PlayerController>();
            playerController.AddCredits((int)-upgrade.cost.credits);
            playerController.AddResearchPoints(-upgrade.cost.researchPoints);
            await playerController.SavePlayerData();
    
            // Списываем ресурсы
            if (upgrade.cost.resources != null)
            {
                foreach (var resourceCost in upgrade.cost.resources)
                {
                    var resourceType = ResourceManager.GetResourceTypeByName(resourceCost.Key);
                    _resourceManager.AddResource(resourceType, -resourceCost.Value);
                }
                
                await _resourceManager.SaveResources(); // Сохраняем при изменении
            }
    
            // Применяем эффект апгрейда
            ApplyUpgrade(upgrade.type, department);
    
            ServiceLocator.Get<UIController>()
                .ShowPopupMessage("Congrats", $"You've purchased a new upgrade - {upgrade.displayName}.");
            // Помечаем апгрейд как купленный
            _purchasedUpgrades[upgrade.upgradeId] = true;
        }
        else
        {
            Debug.LogWarning($"Ошибка покупки {upgradeId}. Недостаточно средств или ресурсов.");
            ServiceLocator.Get<UIController>()
                .ShowPopupMessage("Error", $"Can't purchase upgrade - {upgrade.displayName}. Not enough credits or resources available.");
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

    private bool CheckUpgradeValue(Department department, UpgradeDataSO.UpgradeEntry upgrade)
    {
        var stationBlock = _stationController.StationBlocks
            .FirstOrDefault(block => block.GetBlockType() == department);
        var stationBlockData = _stationController.StationData.DepartmentData
            .FirstOrDefault(block => block.Key == department);
    
        switch (upgrade.type)
        {
            case UpgradeDataSO.UpgradeType.DepartmentUnlock:
                if (_stationController.StationData.IsUnlocked(department)) return false;
                break;
            case UpgradeDataSO.UpgradeType.DepartmentMaxCrew:
                if (stationBlockData.Value.MaxCrewUnlocked >= upgrade.value) return false;
                break;
            case UpgradeDataSO.UpgradeType.StationMaxCrew:
                if (_stationController.StationData.MaxCrew.Value >= upgrade.value) return false;
                break;
            case UpgradeDataSO.UpgradeType.DepartmentCrewHire:
                if (stationBlock != null 
                    && (_stationController.StationData.MaxCrew.Value <= stationBlockData.Value.CurrentCrewHired
                    || stationBlockData.Value.MaxCrewUnlocked < upgrade.value
                    || stationBlockData.Value.CurrentCrewHired != upgrade.value - 1))
                    return false;
                break;
            case UpgradeDataSO.UpgradeType.DepartmentMaxWorkstations:
                if (stationBlock != null && stationBlockData.Value.WorkBenchesMax >= upgrade.value) return false;
                break;
            case UpgradeDataSO.UpgradeType.DepartmentWorkstationAdd:
                if (stationBlock != null && stationBlockData.Value.WorkBenchesInstalled >= stationBlockData.Value.WorkBenchesMax) return false;
                break;
            default:
                Debug.LogError($"UpgradeService: Проверка для типа апгрейда {upgrade.type} не реализована.");
                return true;
        }

        return true;
    }

    public void Dispose()
    {
        _disposables.Clear();
    }
}