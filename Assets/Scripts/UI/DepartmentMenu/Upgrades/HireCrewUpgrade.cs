using System.Linq;

public class HireCrewUpgrade : BuyUpgradeButton
{
    protected override void Initialize()
    {
        var upgrade = ServiceLocator.Get<DataLibrary>().upgradeData.GetUpgradeById(upgradeId);
        var stationController = ServiceLocator.Get<StationController>();

        var departmentData = stationController.StationData.DepartmentData;
        var isUpgradeAvailable = false;
        if (departmentData.TryGetValue(_department, out var data))
        {
            isUpgradeAvailable = data.CurrentCrewHired == upgrade.value - 1 
                                 && stationController.StationData.IsUnlocked(_department);
        }

        gameObject.SetActive(isUpgradeAvailable);
    }
}
