using System.Linq;
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

        if (stationController.StationData != null && stationController.StationData.departmentData.ContainsKey(department))
        {
            var departmentData = stationController.StationData.departmentData[department];
            crewLabel.text = $"Department crew: {departmentData.CurrentCrewHired}/{departmentData.MaxCrewUnlocked}";
            workbenchesLabel.text = $"Workbenches: {departmentData.WorkBenchesLevelUnlocked}/{departmentData.WorkBenchesLevelUnlocked}";
            energyConsumptionLabel.text = $"Energy consumption: {GetBlockEnergyConsumption(department)}";
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

    private float GetBlockEnergyConsumption(Department department)
    {
        StationBlockController blockController = stationController.StationBlocks.FirstOrDefault(block => block.GetBlockType() == department);
        float result = 0f;
        if (blockController != null && blockController.EnergyController != null)
        {
            result = blockController.EnergyController.currentEnergyConsumption.Value;
        }
        return result;
    }

    private float GetBlockMood()
    {
        return 0f;
    }
    
}
