using System.Linq;
using Controllers;
using TMPro;
using UnityEngine;

public class DepartmentInfoPanelViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text crewLabel;
    [SerializeField] private TMP_Text workbenchesLabel;
    [SerializeField] private TMP_Text energyConsumptionLabel;
    [SerializeField] private TMP_Text moodLabel;
    private StationController stationController;

    public void Initialization(Department department)
    {
        stationController = ServiceLocator.Get<StationController>();

        if (stationController.StationData != null && stationController.StationData.DepartmentData.ContainsKey(department))
        {
            var departmentData = stationController.StationData.DepartmentData[department];
            crewLabel.text = $"Department crew: {departmentData.CurrentCrewHired}/{departmentData.MaxCrewUnlocked}";
            workbenchesLabel.text = $"Workbenches: {departmentData.WorkStationsInstalled}/{departmentData.WorkStationsMax}";
            energyConsumptionLabel.text = $"Energy consumption: {GetBlockEnergyConsumption(department)}";
            moodLabel.text = $"Department mood: {GetBlockMood(department)}";
        }
        else
        {
            crewLabel.text = "Department crew: N/A";
            workbenchesLabel.text = "Workbenches: N/A"; 
            energyConsumptionLabel.text = "Energy consumption: N/A";
            moodLabel.text = "Department mood: N/A"; 
        }
    }

    private float GetBlockEnergyConsumption(Department department)
    {
        float result = 0f;
        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        if (energyService != null && department != null)
        {
            DepartmentEnergyInfo energyInfo = energyService.GetDepartmentEnergyInfo(department);
            result = energyInfo.Consumption;
        }
        return result;
    }

    private float GetBlockMood(Department department)
    {
        StationMoodService stationMoodService = ServiceLocator.Get<StationMoodService>();
        var mood = stationMoodService.GetDepartmentMoodInfo(department);
        return mood.MoodEffect;
    }
    
}
