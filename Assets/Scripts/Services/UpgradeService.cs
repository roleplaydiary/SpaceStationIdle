using System;
using System.Linq;
using UniRx;
using UnityEngine;

public class UpgradeService : IDisposable
{
    private readonly StationController _stationController;
    private readonly UpgradeDataSO _upgradeDataSO;
    private readonly ReactiveDictionary<string, bool> _purchasedUpgrades = new ReactiveDictionary<string, bool>();
    public IReadOnlyReactiveDictionary<string, bool> PurchasedUpgrades => _purchasedUpgrades;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public UpgradeService()
    {
        _stationController = ServiceLocator.Get<StationController>();
        _upgradeDataSO = ServiceLocator.Get<DataLibrary>().upgradeData;
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