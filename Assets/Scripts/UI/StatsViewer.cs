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
        

        CreditsValueInitialize();
        EnergyValueInitialize();
        MoodValueInitialize();
    }
    
    private void CreditsValueInitialize()
    {
        playerController = ServiceLocator.Get<PlayerController>();
        
        if (playerController != null && creditsText != null)
        {
            playerController.GetPlayerData().playerCredits.Subscribe(credits =>
            {
                creditsText.text = $"Credits: {Math.Round(credits)}";
            }).AddTo(this);
        }
    }

    private void EnergyValueInitialize()
    {
        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        if (energyService != null)
        {
            energyService.CurrentStationEnergy.Subscribe(value =>
            {
                energyText.text = $"Energy: {value}";
            }).AddTo(this);
        }
    }

    private void MoodValueInitialize()
    {
        stationData = ServiceLocator.Get<StationController>().StationData;
        if (stationData != null)
        {
            stationData.crewMood.Subscribe(mood =>
            {
                currentMoodText.text = $"Mood: {mood}";
            }).AddTo(this);
        }
    }
}
