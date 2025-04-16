using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class StationBlockController : MonoBehaviour
{
    [SerializeField] protected List<WorkBenchController> workBenches;
    [SerializeField] protected StationBlockDataSO stationBlockDataSo;
    protected List<GameObject> crewMembers = new List<GameObject>();
    public Department GetBlockType() {return stationBlockDataSo.BlockType;}
    
    protected StationBlockData blockData;
    
    public virtual void BlockInitialization(StationBlockData _blockData)
    {
        // blockData = _blockData;
        // BenchesInitialization();
        // CrewInitialization();
    }

    protected virtual void BenchesInitialization()
    {
        // if (blockData.WorkBenchesLevelUnlocked == 0)
        //     return;
        //
        // for (int i = 0; i < blockData.WorkBenchesLevelUnlocked; i++)
        // {
        //     workBenches[i].gameObject.SetActive(true);
        // }
    }

    protected virtual void CrewInitialization()
    {
        // if(blockData.MaxCrewUnlocked == 0)
        //     return;
        //
        // if (blockData.CurrentCrewHired == 0)
        //     return;
        //
        // for (int i = 0; i < blockData.CurrentCrewHired; i++)
        // {
        //     var newCrewMember = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[0], transform);
        //     crewMembers.Add(newCrewMember);
        // }
    }
}