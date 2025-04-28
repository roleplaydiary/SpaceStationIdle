using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private StationController stationController;
    [SerializeField] private PlayerController playerController;
    
    [SerializeField] private DataLibrary dataLibrary;
    
    public Subject<bool> OnGameInitialized = new Subject<bool>();

    private async void Start()
    {
        await GameInitialization();
    }

    private async Task GameInitialization()
    {
        ServiceLocator.Get<UIController>().LoadingScreenShow();
        ServiceLocator.Register(dataLibrary);
        
        await ServiceLocator.Get<CloudController>().Autentication();
        await stationController.StationInitialize();
        stationController.BlockCrewInitialize();
        await ResourceManagerInitialize();// Должно инициализироваться перед игроком, чтобы не инициализироваться дважды
        await playerController.PlayerInitialization();
        
        ServiceLocator.Get<WorldDepartmentsButtonsController>().Initialize();
        await ServiceLocator.Get<AFKController>().CheckAFKProduction(); // обязательно после инициализации игрока
        UpgradeServiceInitialize();
        ServiceLocator.Get<StatsViewer>().StatsIninitlize();
        ServiceLocator.Get<CrewService>().Initialize();

        OnGameInitialized.OnNext(true);
        await ServiceLocator.Get<PlayerController>().SavePlayerData();// Обновляем время захода в игру для AFK контроллера
        ServiceLocator.Get<AudioManager>().PlayBackgroundMusic(dataLibrary.soundLibrary.backgroundMusicTracks[0]);//TODO: Стартуем случайную композицию
        ServiceLocator.Get<UIController>().LoadingScreenHide();
    }

    private async Task ResourceManagerInitialize()
    {
        ResourceManager resourceManager = new ResourceManager();
        ServiceLocator.Register(resourceManager);
        await resourceManager.InitializeAsync();
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
