using UniRx;
using UnityEngine;

public class BridgeBlockController : StationBlockController
{
    public ReactiveProperty<int> crewAtWork { get; private set; }
    public ReactiveProperty<int> crewAtRest { get; private set; }
    public ReactiveProperty<int> crewAtIdle { get; private set; }
    
    public override void BlockInitialization(StationBlockData _blockData)
    {
        crewAtWork = new ReactiveProperty<int>(0);
        crewAtRest = new ReactiveProperty<int>(0);
        crewAtIdle = new ReactiveProperty<int>(0);
        
        blockData = _blockData;
        BenchesInitialization();
        CrewInitialization();
    }
    
    protected override void BenchesInitialization()
    {
        if (blockData.WorkBenchesLevelUnlocked == 0)
            return;
        
        for (int i = 0; i < blockData.WorkBenchesLevelUnlocked; i++)
        {
            workBenches[i].gameObject.SetActive(true);
        }
    }
    
    protected override void CrewInitialization()
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
        
        crewAtWork.Value = 0;
        for (int i = 0; i < crewMembers.Count; i++)
        {
            var crewMemberStateMachine = crewMembers[i].GetComponent<CharacterController>();
            crewMemberStateMachine.GoToWork(workBenches[i].GetWorkPosition());
            crewAtWork.Value ++;
        }
    }

    public void SendCrewToRest(int crewCount)
    {
        
    }
    
    public void SendCrewToIdle(int crewCount)
    {
        
    }
}
