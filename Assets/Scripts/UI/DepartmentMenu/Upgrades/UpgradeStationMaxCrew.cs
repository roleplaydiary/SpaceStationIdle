public class UpgradeStationMaxCrew : BuyUpgradeButton
{
    protected override void Initialize()
    {
        var upgrade = ServiceLocator.Get<DataLibrary>().upgradeData.GetUpgradeById(upgradeId);
        var stationController = ServiceLocator.Get<StationController>();

        var isUpgradeAvailable = false;

        var currentCrewMax = stationController.StationData.MaxCrew.Value;
        if (currentCrewMax == upgrade.value - 5) // TODO: продумать и переделать подход, этот вариант временный и очень плохой
        {
            isUpgradeAvailable = true;
        }

        gameObject.SetActive(isUpgradeAvailable);
    }
}
