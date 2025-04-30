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
        var crewService = ServiceLocator.Get<CrewService>();
        crewService.OnWorkingCrewValueUpdate.Subscribe(Initialize).AddTo(this);
        
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            var uiController = ServiceLocator.Get<UIController>();
            uiController.TradeScreenShow();
        }).AddTo(this);
        
        var stationController = ServiceLocator.Get<StationController>();
        var workingCrew = stationController.StationData.DepartmentData[Department.Cargo].CrewAtWork;
        Initialize(workingCrew);
    }

    private void Initialize(int workingCrew)
    {
        if (workingCrew > 0)
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
