using System;
using TMPro;
using UniRx;
using UnityEngine;

public class StatsViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text currentMoodText;
    
    private PlayerController playerController;
    private StationData stationData;
    
    private void Awake()
    {
        ServiceLocator.Register(this);
    }
    
    public void StatsIninitlize()
    {
        playerController = ServiceLocator.Get<PlayerController>();
        
        if (playerController != null && creditsText != null)
        {
            playerController.GetPlayerData().playerCredits.Subscribe(credits =>
            {
                creditsText.text = $"Credits: {Math.Round(credits)}";
            }).AddTo(this);
        }
        
        stationData = ServiceLocator.Get<StationController>().StationData;
        if (stationData != null)
        {
            stationData.crewMood.Subscribe(mood =>
            {
                currentMoodText.text = $"Mood: {mood}";
            }).AddTo(this);
            
            stationData.stationEnergy.Subscribe(energy =>
            {
                energyText.text = $"Energy: {energy}";
            }).AddTo(this);
        }
    }
}
