using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private StationController stationController;
    [SerializeField] private PlayerController playerController;
    
    [SerializeField] private DataLibrary dataLibrary;

    private async void Start()
    {
        await GameInitialization();
    }

    private async Task GameInitialization()
    {
        ServiceLocator.Get<UIController>().LoadingScreenShow();
        ServiceLocator.Register(dataLibrary);
        
        await ServiceLocator.Get<CloudController>().Autentication();
        await stationController.StationInitializate();
        ResourceManagerInitialize();// Должно инициализироваться перед игроком, чтобы не инициализироваться дважды
        await playerController.PlayerInitialization();
        UpgradeServiceInitialize();
        ServiceLocator.Get<StatsViewer>().StatsIninitlize();

        await ServiceLocator.Get<PlayerController>().SavePlayerData();// Обновляем время захода в игру для AFK контроллера
        ServiceLocator.Get<UIController>().LoadingScreenHide();
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
