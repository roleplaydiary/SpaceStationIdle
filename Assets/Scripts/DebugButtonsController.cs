using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonsController : MonoBehaviour
{
    [SerializeField] private Button saveResources;
    [SerializeField] private Button addCredits;
    [SerializeField] private Button addResearchPoints;
    [SerializeField] private Button addResource;
    [SerializeField] private Button testButton;
    [SerializeField] private Button eventTestButton;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {

        saveResources.OnClickAsObservable().Subscribe(async button =>
        {
            ResourceManager resourceManager = ServiceLocator.Get<ResourceManager>();
            await ServiceLocator.Get<CloudController>().SaveResources(resourceManager.CurrentResources.Value);
        }).AddTo(_disposables);
        
        testButton.OnClickAsObservable().Subscribe(async button =>
        {
            var currentTime = await OnlineTimeService.GetUTCTimeAsync();
            Debug.Log($"Current time is {currentTime}");
        }).AddTo(_disposables);
        
        eventTestButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<StationEventsController>().TestEvent();
        }).AddTo(_disposables);

        AddCredits();
        AddResearchPoints();
        AddResource();
    }

    private void AddCredits()
    {
        addCredits.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerController playerController = ServiceLocator.Get<PlayerController>();
            playerController.AddCredits(100);
            
            UIController uiController = ServiceLocator.Get<UIController>();
            uiController.PopupMessageShow("Debug UI", "Вы получили 100 кредитов, радуйтесь.");
        }).AddTo(_disposables);
    }

    private void AddResearchPoints()
    {
        addResearchPoints.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerController playerController = ServiceLocator.Get<PlayerController>();
            playerController.AddResearchPoints(100);
            
            UIController uiController = ServiceLocator.Get<UIController>();
            uiController.PopupMessageShow("Debug UI", "Вы получили 100 RP, радуйтесь.");
        }).AddTo(_disposables);
    }

    private void AddResource()
    {
        addResource.OnClickAsObservable().Subscribe(_ =>
        {
            ResourceManager resourceManager = ServiceLocator.Get<ResourceManager>();
            //cargoController.ProduceRandomResource();
            resourceManager.AddResource(ResourceType.Phoron, 1f);
        }).AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
