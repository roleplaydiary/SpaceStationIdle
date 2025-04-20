using TMPro;
using UnityEngine;
using UniRx;

public class DebugUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text maxCrewText;
    [SerializeField] private BridgeBlockController bridge;

    private PlayerController playerController;
    private StationData stationData;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    private void Update()
    {
        // var currentCrew = stationController.GetStationData().departmentData[Department.Bridge].CurrentCrewHired;
        // if(currentCrew != null)
        //     currentCrewText.text = $"CurrentCrew: {currentCrew}";
    }

    public void DebugUIInitialize()
    {
        playerController = ServiceLocator.Get<PlayerController>();
        
        if (playerController != null)
        {
        }
    }

    // Метод OnDestroy для очистки подписок (хотя AddTo(this) должен это делать)
    private void OnDestroy()
    {
        // Все подписки, добавленные через .AddTo(this), будут автоматически отписаны.
    }
}