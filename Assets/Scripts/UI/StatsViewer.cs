using System;
using TMPro;
using UniRx;
using UnityEngine;

public class StatsViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private TMP_Text creditsProductionText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text currentMoodText;
    [SerializeField] private TMP_Text researchPointsText;
    [SerializeField] private TMP_Text researchPointsProductionText;
    
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
        ResearchPointsValueInitialize();
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
        
        var stationContoller = ServiceLocator.Get<StationController>();
        if (stationContoller != null && creditsProductionText != null)
        {
            var productionValue = stationContoller.GetStationCreditProductionValue();
            var text = productionValue >= 0 ? "+" + productionValue : "-" + productionValue;
            creditsProductionText.text = $"{text}";
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
        var StationMoodService = ServiceLocator.Get<StationMoodService>();
        if (StationMoodService != null)
        {
            StationMoodService.CurrentStationMood.Subscribe(mood =>
            {
                currentMoodText.text = $"Mood: {mood}";
            }).AddTo(this);
        }
    }

    private void ResearchPointsValueInitialize()
    {
        playerController = ServiceLocator.Get<PlayerController>();
        
        if (playerController != null && researchPointsText != null)
        {
            playerController.GetPlayerData().researchPoints.Subscribe(credits =>
            {
                researchPointsText.text = $"RP: {Math.Round(credits)}";
            }).AddTo(this);
        }
        
        var stationContoller = ServiceLocator.Get<StationController>();
        if (stationContoller != null && researchPointsProductionText != null)
        {
            var productionValue = stationContoller.GetStationResearchProductionValue();
            var text = productionValue >= 0 ? "+" + productionValue : "-" + productionValue;
            creditsProductionText.text = $"{text}";
            researchPointsProductionText.text = $"{text}";
        }
    }
}
