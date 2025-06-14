using System;
using Controllers;
using Services;
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
    [SerializeField] private Color negativeValueColor;
    [SerializeField] private Color positiveValueColor;
    [SerializeField] private Color normalValueColor;
    
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
                creditsText.text = $"<size=200%><sprite name=\"coins-icon\"></size>: {Utils.FormatBigNumber(credits)}";
            }).AddTo(this);
        }
        
        var stationContoller = ServiceLocator.Get<StationController>();
        var crewService = ServiceLocator.Get<CrewService>();
        crewService.OnWorkingCrewValueUpdate.Subscribe(crewAtWork =>
        {
            if (stationContoller != null && creditsProductionText != null)
            {
                var productionValue = stationContoller.GetStationCreditProductionValue();
                var text = productionValue >= 0 ? "+" + productionValue : "-" + productionValue;
                creditsProductionText.text = $"{text}";
                LabelColorUpdate(creditsProductionText, productionValue);
            }
        }).AddTo(this);
    }

    private void EnergyValueInitialize()
    {
        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        if (energyService != null)
        {
            energyService.CurrentStationEnergy.Subscribe(value =>
            {
                energyText.text = $"<size=200%><sprite name=\"energy-icon\"></size>: {value}";
                LabelColorUpdate(energyText, value);
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
                currentMoodText.text = $"<size=200%><sprite name=\"mood-icon\"></size>: {mood}";
                LabelColorUpdate(currentMoodText, mood);
            }).AddTo(this);
        }
    }

    private void ResearchPointsValueInitialize()
    {
        playerController = ServiceLocator.Get<PlayerController>();
        
        if (playerController != null && researchPointsText != null)
        {
            playerController.GetPlayerData().researchPoints.Subscribe(researchPoints =>
            {
                researchPointsText.text = $"<size=200%><sprite name=\"research-icon\"></size>: {Utils.FormatBigNumber(researchPoints)}";
                LabelColorUpdate(researchPointsText, researchPoints);
            }).AddTo(this);
        }
        
        var crewService = ServiceLocator.Get<CrewService>();
        var stationContoller = ServiceLocator.Get<StationController>();
        crewService.OnWorkingCrewValueUpdate.Subscribe(crewAtWork =>
        {
            if (stationContoller != null && researchPointsProductionText != null)
            {
                var productionValue = stationContoller.GetStationResearchProductionValue();
                var text = productionValue >= 0 ? "+" + productionValue : "-" + productionValue;
                researchPointsProductionText.text = $"{text}";
                LabelColorUpdate(researchPointsProductionText, productionValue);
            }
        }).AddTo(this);
    }
    
    private void LabelColorUpdate(TMP_Text label, float value)
    {
        if (value < 0)
        {
            label.color = negativeValueColor;
        }
        else if (value > 0)
        {
            label.color = positiveValueColor;
        }
        else if (value == 0)
        {
            label.color = normalValueColor;
        }
    }
}
