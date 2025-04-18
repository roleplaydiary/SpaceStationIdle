using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private StationController stationController;
    [SerializeField] private PlayerController playerController;
    
    [SerializeField] private DataLibrary dataLibrary;
    [SerializeField] private GameObject loadingImage;

    private async void Start()
    {
        ServiceLocator.Register(dataLibrary);
        await GameInitialization();
    }

    private async Task GameInitialization()
    {
        loadingImage.SetActive(true);
        await ServiceLocator.Get<CloudController>().Autentication();
        await playerController.PlayerInitialization();
        await stationController.StationInitializate();
        ServiceLocator.Get<DebugUIController>().DebugUIInitialize();
        ServiceLocator.Get<StatsViewer>().StatsIninitlize();
        loadingImage.SetActive(false);
    }

    public void TestSaveStationButton()
    {
        TestSaveStation();
    }
    private async Task TestSaveStation()
    {
        var stationData = ServiceLocator.Get<StationController>().GetStationData();
        await ServiceLocator.Get<CloudController>().SaveStationData(stationData);
    }
}
