using TMPro;
using UnityEngine;


public class CargoProductionViewer : DepartmentProductionViewer
{
    [SerializeField] private TMP_Text productionLabel;
    public override void Initialize()
    {
        var station = ServiceLocator.Get<StationController>();
        foreach (var block in station.StationBlocks)
        {
            if (block.GetBlockType() == Department.Cargo)
            {
                productionLabel.text = $"Credits: +{block.GetProductionValue()}/min";
                break;
            }
        }
    }
    
    //TODO: Отображать вероятность получения ресурсов
}
