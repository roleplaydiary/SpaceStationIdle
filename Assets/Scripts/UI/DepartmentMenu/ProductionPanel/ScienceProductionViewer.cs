using Controllers;
using TMPro;
using UnityEngine;

public class ScienceProductionViewer : DepartmentProductionViewer
{
    [SerializeField] private TMP_Text productionLabel;
    public override void Initialize()
    {
        var station = ServiceLocator.Get<StationController>();
        foreach (var block in station.StationBlocks)
        {
            if (block.GetBlockType() == Department.Science)
            {
                productionLabel.text = $"Research points: {block.GetProductionValue()}/min";
                
                break;
            }
        }
    }
}
