using TMPro;
using UnityEngine;

public class BridgeProductionViewer : DepartmentProductionViewer
{
    [SerializeField] private TMP_Text productionLabel;
    public override void Initialize()
    {
        var station = ServiceLocator.Get<StationController>();
        foreach (var block in station.StationBlocks)
        {
            if (block.GetBlockType() == Department.Bridge)
            {
                productionLabel.text = $"Credits: {block.GetProductionValue()}";
                break;
            }
        }
    }
}
