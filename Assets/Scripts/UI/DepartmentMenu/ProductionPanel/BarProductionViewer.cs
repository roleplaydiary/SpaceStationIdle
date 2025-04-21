using TMPro;
using UnityEngine;

public class BarProductionViewer : DepartmentProductionViewer
{
    [SerializeField] private TMP_Text productionLabel;
    public override void Initialize()
    {
        var station = ServiceLocator.Get<StationController>();
        foreach (var block in station.StationBlocks)
        {
            if (block.GetBlockType() == Department.Bar)
            {
                productionLabel.text = $"Station mood: +{block.GetProductionValue()}/total";
                break;
            }
        }
    }
}
