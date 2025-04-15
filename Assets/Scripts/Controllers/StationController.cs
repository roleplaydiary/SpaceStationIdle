using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<StationBlockController> stationBlocks;
    [SerializeField] private StationData stationData;
    

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public async Task StationInitializate()
    {
        // var testData = new StationData
        // {
        //     departmentData = new Dictionary<Department, StationBlockData>
        //     {
        //         { Department.Bridge, new StationBlockData { WorkBenchesLevelUnlocked = 1, MaxCrewUnlocked = 1, CurrentCrewHired = 1 } },
        //         { Department.RND, new StationBlockData { WorkBenchesLevelUnlocked = 1, MaxCrewUnlocked = 0, CurrentCrewHired = 0 } },
        //     }
        // };
        var loadData = await ServiceLocator.Get<CloudController>().LoadStationData();
        if (loadData == null || loadData.departmentData.Count == 0)
        {
            loadData = new StationData();
            loadData.Unlock(Department.Bridge); // Разблокируем Bridge
            loadData.SetWorkbenchesLevelUnlocked(Department.Bridge, 1);
            loadData.SetMaxCrewUnlocked(Department.Bridge, 1);
            loadData.SetCurrentCrewHired(Department.Bridge, 1);
        }
        BlocksInitialize(loadData);
    }
    
    private void BlocksInitialize(StationData stationData)
    {
        this.stationData = stationData;
        foreach (var block in stationBlocks)
        {
            var blockType = block.GetBlockType();
            if (this.stationData.IsUnlocked(blockType))
            {
                block.BlockInitialization(this.stationData.departmentData[blockType]);
            }
            else
            {
                block.gameObject.SetActive(false);
            }
        }
    }

    public StationData GetStationData()
    {
        return stationData;
    }
}
