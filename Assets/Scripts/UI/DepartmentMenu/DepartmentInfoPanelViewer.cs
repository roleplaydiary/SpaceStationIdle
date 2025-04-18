using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DepartmentInfoPanelViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text crewLabel;
    [SerializeField] private TMP_Text workbenchesLabel;
    [SerializeField] private TMP_Text energyConsumptionLabel;
    [SerializeField] private TMP_Text moodLabel;
    

    public void Initialization(Department department)
    {
        StationData stationData = ServiceLocator.Get<StationController>().StationData;

        if (stationData != null && stationData.departmentData.ContainsKey(department))
        {
            var departmentData = stationData.departmentData[department];
            crewLabel.text = $"Department crew: {departmentData.CurrentCrewHired}/{departmentData.MaxCrewUnlocked}";
            workbenchesLabel.text = $"Workbenches: {departmentData.WorkBenchesLevelUnlocked}/{departmentData.WorkBenchesLevelUnlocked}";
            energyConsumptionLabel.text = $"Energy consumption: {GetBlockEnergyConsumption()}";
            moodLabel.text = $"Department mood: {GetBlockMood()}";
        }
        else
        {
            crewLabel.text = "Department crew: N/A";
            workbenchesLabel.text = "Workbenches: N/A"; 
            energyConsumptionLabel.text = "Energy consumption: N/A";
            moodLabel.text = "Department mood: N/A"; 
        }
    }

    private float GetBlockEnergyConsumption()
    {
        return 0f;
    }

    private float GetBlockMood()
    {
        return 0f;
    }
    
}
