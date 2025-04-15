using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class StationBlockController : MonoBehaviour
{
    [SerializeField] private List<WorkBenchController> workBenches;
    [SerializeField] private StationBlockDataSO stationBlockDataSo;
    public Department GetBlockType() {return stationBlockDataSo.BlockType;}
    private StationBlockData blockData;

    private List<GameObject> crewMembers = new List<GameObject>();
    [SerializeField, ReadOnly]
    private int crewAtWork = 0;
    
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
            crewMembers.Add(newCrewMember);
        }
    }

    public void SendCrewToWork(int crewCount)
    {
        if (blockData.CurrentCrewHired == 0)
        {
            Debug.Log("недостаточно работников для выполнения");
            return;
        }

        if (crewCount > blockData.CurrentCrewHired)
        {
            Debug.Log("У вас нет столько сотрудников");
            return;
        }
        
        crewAtWork = 0;
        for (int i = 0; i < crewMembers.Count; i++)
        {
            var crewMemberStateMachine = crewMembers[i].GetComponent<CharacterController>();
            crewMemberStateMachine.GoToWork(workBenches[i].GetWorkPosition());
            crewAtWork++;
        }
    }

    public void SendCrewToRest(int crewCount)
    {
        
    }
    
    public void SendCrewToIdle(int crewCount)
    {
        
    }
}