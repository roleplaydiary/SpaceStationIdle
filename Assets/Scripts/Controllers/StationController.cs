using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<StationBlockController> stationBlocks;
    
    public void StationInitializate()
    {
        BlocksInitialize();
    }
    
    private void BlocksInitialize()
    {
        var testData = new StationBlockData()
        {
            CurrentCrewHired = 1,
            MaxCrewUnlocked = 1,
            WorkBenchesLevelUnlocked = 2

        };
        
        foreach (var block in stationBlocks)
        {
            block.BlockInitialization(testData);
            // Инициализация каждого блока
            // Количество экипажа в блоке
            // Параметры блока(Апгрейды)
        }
    }
}
