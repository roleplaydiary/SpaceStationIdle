using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private StationController stationController;
    [SerializeField] private PlayerController playerController;
    
    [SerializeField] private DataLibrary dataLibrary;
    
    public readonly BehaviorSubject<bool> OnGameInitialized = new BehaviorSubject<bool>(false);

    private async void Awake()
    {
        await GameInitialization();
    }

    private async Task GameInitialization()
    {
        ServiceLocator.Register(this);
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
        Debug.Log("GameController: GameInitialization OnGameInitialized");
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
}
