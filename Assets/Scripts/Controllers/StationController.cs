using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<StationBlockController> stationBlocks;
    
    public async Task StationInitializate()
    {
        var testData = new StationData
        {
            unlockedDepartments = new List<Department>
            {
                Department.Bridge,
                Department.RND
            }
        };
        await Task.Delay(1000);//TODO: тестовые вводные
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
            if (stationData.IsUnlocked(block.GetBlockType()))
            {
                block.BlockInitialization(testData);
            }
            else
            {
                block.gameObject.SetActive(false);
            }
        }
    }

}
