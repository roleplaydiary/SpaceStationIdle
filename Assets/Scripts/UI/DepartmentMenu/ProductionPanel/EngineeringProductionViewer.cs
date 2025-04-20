using TMPro;
using UnityEngine;

public class EngineeringProductionViewer : DepartmentProductionViewer
{
    [SerializeField] private TMP_Text productionLabel;
    public override void Initialize()
    {
        var station = ServiceLocator.Get<StationController>();
        foreach (var block in station.StationBlocks)
        {
            if (block.GetBlockType() == Department.Engineering)
            {
                productionLabel.text = $"Energy: {block.GetProductionValue()} in total";
                break;
            }
        }
    }
}
