using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonsController : MonoBehaviour
{
    [SerializeField] private Button saveResources;
    [SerializeField] private Button addCredits;
    [SerializeField] private Button addResearchPoints;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {

        saveResources.OnClickAsObservable().Subscribe(async button =>
        {
            ResourceManager resourceManager = ServiceLocator.Get<ResourceManager>();
            await ServiceLocator.Get<CloudController>().SaveResources(resourceManager.GetCurrentResources());
        }).AddTo(_disposables);

        AddCredits();
        AddResearchPoints();
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
        }).AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
