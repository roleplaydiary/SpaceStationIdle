using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonsController : MonoBehaviour
{
    [SerializeField] private Button saveResources;
    [SerializeField] private Button addCredits;
    [SerializeField] private Button addResearchPoints;
    [SerializeField] private Button addResource;
    [SerializeField] private Button tradeButton;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {

        saveResources.OnClickAsObservable().Subscribe(async button =>
        {
            ResourceManager resourceManager = ServiceLocator.Get<ResourceManager>();
            await ServiceLocator.Get<CloudController>().SaveResources(resourceManager.CurrentResources.Value);
        }).AddTo(_disposables);
        
        tradeButton.OnClickAsObservable().Subscribe(async button =>
        {
            ServiceLocator.Get<UIController>().TradeScreenShow();
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
            uiController.ShowPopupMessage("Debug UI", "Вы получили 100 кредитов, радуйтесь.");
        }).AddTo(_disposables);
    }

    private void AddResearchPoints()
    {
        addResearchPoints.OnClickAsObservable().Subscribe(_ =>
        {
            PlayerController playerController = ServiceLocator.Get<PlayerController>();
            playerController.AddResearchPoints(100);
            
            UIController uiController = ServiceLocator.Get<UIController>();
            uiController.ShowPopupMessage("Debug UI", "Вы получили 100 RP, радуйтесь.");
        }).AddTo(_disposables);
    }

    private void AddResource()
    {
        addResource.OnClickAsObservable().Subscribe(_ =>
        {
            ResourceManager resourceManager = ServiceLocator.Get<ResourceManager>();
            //cargoController.ProduceRandomResource();
            //resourceManager.AddResource(ResourceType.Phoron, 0.1);
        }).AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
