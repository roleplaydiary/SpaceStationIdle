using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBlockController : MonoBehaviour
{
    [SerializeField] private List<WorkBenchController> workBenches;
    [SerializeField] private StationBlockDataSO stationBlockDataSo;
    public Department GetBlockType() {return stationBlockDataSo.BlockType;}
    
    private StationBlockData blockData;
    
    public void BlockInitialization(StationBlockData _blockData)
    {
        blockData = _blockData;
        BenchesInitialization();
        CrewInitialization();
    }

    private void BenchesInitialization()
    {
        if (blockData.WorkBenchesLevelUnlocked == 0)
            return;
        
        for (int i = 0; i < blockData.WorkBenchesLevelUnlocked; i++)
        {
            workBenches[i].gameObject.SetActive(true);
        }
    }

    private void CrewInitialization()
    {
        if(blockData.MaxCrewUnlocked == 0)
            return;
        
        if (blockData.CurrentCrewHired == 0)
            return;
        
        for (int i = 0; i < blockData.CurrentCrewHired; i++)
        {
            var newCrewMember = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[0], transform);
        }
    }
}