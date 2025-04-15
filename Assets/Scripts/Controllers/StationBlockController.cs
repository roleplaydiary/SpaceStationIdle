using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBlockController : MonoBehaviour
{
    [SerializeField] private List<GameObject> workBenches = new List<GameObject>();
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
            workBenches[i].SetActive(true);
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
            //Spawn a crew member
        }
    }
}