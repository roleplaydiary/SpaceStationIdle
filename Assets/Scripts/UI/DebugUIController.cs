using TMPro;
using UnityEngine;
using UniRx;

public class DebugUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private TMP_Text currentCrewText;
    [SerializeField] private TMP_Text currentMoodText;
    [SerializeField] private TMP_Text maxCrewText;
    [SerializeField] private TMP_Text bridgeCrewAtWorkText;
    [SerializeField] private TMP_Text bridgeCrewAtRestText;
    [SerializeField] private TMP_Text bridgeCrewAtIdleText;

    [SerializeField] private BridgeBlockController bridge;

    private PlayerController playerController;
    private StationController stationController;

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
        if (playerController != null && creditsText != null)
        {
            playerController.GetPlayerData().playerCredits.Subscribe(credits =>
            {
                creditsText.text = $"Credits: {credits}";
            }).AddTo(this);
            
            playerController.GetPlayerData().crewMood.Subscribe(mood =>
            {
                currentMoodText.text = $"Crew mood: {mood}";
            }).AddTo(this);
        }

        // Подписка на другие ReactiveProperty (примеры)
        if (playerController != null && maxCrewText != null)
        {
            playerController.GetPlayerData().maxCrew.Subscribe(max =>
            {
                maxCrewText.text = $"Max Crew: {max}";
            }).AddTo(this);
        }

        bridge.crewAtWork.Subscribe(i =>
        {
            bridgeCrewAtWorkText.text = $"Crew at work: {i}";
        }).AddTo(this);
        
        bridge.crewAtRest.Subscribe(i =>
        {
            bridgeCrewAtRestText.text = $"Crew at rest: {i}";
        }).AddTo(this);
        
        bridge.crewAtIdle.Subscribe(i =>
        {
            bridgeCrewAtIdleText.text = $"Crew at idle: {i}";
        }).AddTo(this);
    }

    // Метод OnDestroy для очистки подписок (хотя AddTo(this) должен это делать)
    private void OnDestroy()
    {
        // Все подписки, добавленные через .AddTo(this), будут автоматически отписаны.
    }
}