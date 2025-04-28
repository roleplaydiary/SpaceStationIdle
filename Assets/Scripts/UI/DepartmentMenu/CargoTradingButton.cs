using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CargoTradingButton : MonoBehaviour
{
    [SerializeField] private GameObject backgroundActive;
    [SerializeField] private GameObject backgroundInActive;
    [SerializeField] private Button _button;
    private void Start()
    {
        var crewService = ServiceLocator.Get<CrewManager>();
        crewService.workingCrew.ObserveCountChanged().Subscribe(value =>
        {
            Initialize();
        }).AddTo(this);
        
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            var uiController = ServiceLocator.Get<UIController>();
            uiController.TradeScreenShow();
        }).AddTo(this);
        
        Initialize();
    }

    private void Initialize()
    {
        var stationController = ServiceLocator.Get<StationController>();
        if (stationController.StationData.DepartmentData[Department.Cargo].CrewAtWork >= 4)
        {
            backgroundActive.SetActive(true);
            backgroundInActive.SetActive(false);
            _button.interactable = true;
        }
        else
        {
            backgroundActive.SetActive(false);
            backgroundInActive.SetActive(true);
            _button.interactable = false;
        }
    }
}
