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
        ResourceManagerInitialize();
        UpgradeServiceInitialize();
        ServiceLocator.Get<StatsViewer>().StatsIninitlize();
        loadingImage.SetActive(false);
    }

    private void ResourceManagerInitialize()
    {
        ResourceManager resourceManager = new ResourceManager();
        ServiceLocator.Register(resourceManager);
        Debug.Log("ResourceManager зарегистрирован в ServiceLocator.");
    }

    private void UpgradeServiceInitialize()
    {
        UpgradeService upgradeService = new UpgradeService();
        ServiceLocator.Register(upgradeService);

        Debug.Log("UpgradeService зарегистрирован в ServiceLocator.");
    }
    

    public void TestSaveStationButton()
    {
        TestSaveStation();
    }
    private async Task TestSaveStation()
    {
        var stationData = ServiceLocator.Get<StationController>().StationData;
        await ServiceLocator.Get<CloudController>().SaveStationData(stationData);
    }
}
