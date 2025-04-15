using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<StationBlockController> stationBlocks;
    
    public void StationInitializate()
    {
        var testData = new StationData
        {
            unlockedDepartments = new List<Department>
            {
                Department.RND
            }
        };
        
        BlocksInitialize(testData);
    }
    
    private void BlocksInitialize(StationData stationData)
    {
        var testData = new StationBlockData // FOR TESTS
        {
            CurrentCrewHired = 1,
            MaxCrewUnlocked = 2,
            WorkBenchesLevelUnlocked = 1
        };

        foreach (var block in stationBlocks)
        {
            // Проверка: открыт ли соответствующий отдел
            if (stationData.IsUnlocked(block.GetBlockType()))
            {
                block.BlockInitialization(testData);
                // Здесь инициализируем только открытые блоки
            }
            else
            {
                block.gameObject.SetActive(false); // можно отключать закрытые блоки
            }
        }
    }

}
