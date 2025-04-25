public class UnlockDepartmentUpgrade : BuyUpgradeButton
{
    protected override void Initialize()
    {
        //var upgrade = ServiceLocator.Get<DataLibrary>().upgradeData.GetUpgradeById(upgradeId);
        var stationController = ServiceLocator.Get<StationController>();

        var isUpgradeAvailable = !stationController.StationData.IsUnlocked(_department);
        gameObject.SetActive(isUpgradeAvailable);
    }
}
