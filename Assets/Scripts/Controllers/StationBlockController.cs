using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StationBlockController : MonoBehaviour
{
    [SerializeField] protected Transform workBenchesParent;
    [SerializeField] protected Transform idlePositionParent;
    [SerializeField] protected StationBlockDataSO stationBlockDataSo;
    protected List<CharacterController> crewMembers = new List<CharacterController>();
    public Department GetBlockType() { return stationBlockDataSo.BlockType; }

    protected StationBlockData blockData;

    public ReactiveProperty<int> crewAtWork { get; protected set; }
    public ReactiveProperty<int> crewAtRest { get; protected set; }
    public ReactiveProperty<int> crewAtIdle { get; protected set; }

    public ReactiveCollection<CharacterController> workingCrew = new ReactiveCollection<CharacterController>();
    protected ReactiveCollection<CharacterController> restingCrew = new ReactiveCollection<CharacterController>();
    protected ReactiveCollection<CharacterController> idleCrew = new ReactiveCollection<CharacterController>();
    protected ReactiveCollection<CharacterController> allCrewMembers = new ReactiveCollection<CharacterController>();

    protected List<Transform> idlePositionList = new List<Transform>();
    public List<WorkBenchController> workBenchesList = new List<WorkBenchController>();

    protected int targetCrewAtWork = 0;
    protected int targetCrewAtRest = 0;

    protected StationController stationController;
    public DepartmentEnergyController EnergyController { get; private set; }

    public virtual void BlockInitialization(StationBlockData _blockData)
    {
        crewAtWork = new ReactiveProperty<int>(0);
        crewAtRest = new ReactiveProperty<int>(0);
        crewAtIdle = new ReactiveProperty<int>(0);

        blockData = _blockData;
        stationController = ServiceLocator.Get<StationController>();

        InitializeLists();
        BenchesInitialization();
        CrewInitialization();
        RestoreCrewAssignment();

        EnergyController = gameObject.GetComponent<DepartmentEnergyController>();
        EnergyController.Initialize(this);

        crewAtWork.Subscribe(_crewAtWork =>
        {
            blockData.CrewAtWork = _crewAtWork;
            // Теперь DepartmentEnergyController сам подпишется на workingCrew
        }).AddTo(this);

        crewAtRest.Subscribe(_crewAtRest =>
        {
            blockData.CrewAtRest = _crewAtRest;
        }).AddTo(this);
    }

    protected virtual void BenchesInitialization()
    {
        if (blockData.WorkBenchesLevelUnlocked == 0)
            return;

        if (workBenchesParent != null)
        {
            for (int i = 0; i < blockData.WorkBenchesLevelUnlocked && i < workBenchesParent.childCount; i++)
            {
                WorkBenchController workBenchController = workBenchesParent.GetChild(i).GetComponent<WorkBenchController>();
                if (workBenchController != null)
                {
                    workBenchesList.Add(workBenchController);
                    workBenchController.gameObject.SetActive(true);
                }
            }
        }
    }

    protected virtual void CrewInitialization()
    {
        if (blockData.MaxCrewUnlocked == 0 || blockData.CurrentCrewHired == 0)
            return;

        for (int i = 0; i < blockData.CurrentCrewHired; i++)
        {
            var newCrewMember = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[(int)GetBlockType()], transform);
            CharacterController crewController = newCrewMember.GetComponent<CharacterController>();
            crewMembers.Add(crewController);
            allCrewMembers.Add(crewController);
        }
    }

    protected void ClearCrewLists()
    {
        workingCrew.Clear();
        restingCrew.Clear();
        idleCrew.Clear();
    }

    protected void UpdateCrewCounts()
    {
        crewAtWork.Value = workingCrew.Count;
        crewAtRest.Value = restingCrew.Count;
        crewAtIdle.Value = idleCrew.Count;
    }

    protected void DistributeCrew()
    {
        ClearCrewLists();

        for (int i = 0; i < Mathf.Min(targetCrewAtWork, crewMembers.Count); i++)
        {
            if (i < workBenchesList.Count && workBenchesList[i] != null)
            {
                crewMembers[i].GoToWork(workBenchesList[i].GetWorkPosition());
                workingCrew.Add(crewMembers[i]);
            }
            else if (workingCrew.Count < targetCrewAtWork)
            {
                if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
                {
                    crewMembers[i].GotoIdle(GetAvailableIdlePosition());
                    idleCrew.Add(crewMembers[i]);
                }
            }
        }

        int restedCount = 0;
        for (int i = 0; i < crewMembers.Count && restedCount < targetCrewAtRest; i++)
        {
            if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
            {
                crewMembers[i].GoToRest(GetAvailableRestPosition());
                restingCrew.Add(crewMembers[i]);
                restedCount++;
            }
        }

        foreach (var member in crewMembers)
        {
            if (workingCrew.All(c => c != member) && restingCrew.All(c => c != member) && idleCrew.All(c => c != member))
            {
                member.GotoIdle(GetAvailableIdlePosition());
                idleCrew.Add(member);
            }
        }

        UpdateCrewCounts();
        OnCrewDistributed();
    }

    private async void SaveBlockData()
    {
        if (stationController != null)
        {
            Department currentDepartment = GetBlockType();
            await ServiceLocator.Get<CloudController>().SaveDepartmentData(blockData, currentDepartment);
        }
        else
        {
            Debug.LogError("StationController не найден в ServiceLocator!");
        }
    }

    public void SendCrewToWork(int crewCount)
    {
        targetCrewAtWork = Mathf.Clamp(crewCount, 0, blockData.CurrentCrewHired);
        targetCrewAtRest = Mathf.Clamp(targetCrewAtRest, 0, blockData.CurrentCrewHired - targetCrewAtWork);
        DistributeCrew();
        SaveBlockData();
    }

    public void SendCrewToRest(int crewCount)
    {
        targetCrewAtRest = Mathf.Clamp(crewCount, 0, blockData.CurrentCrewHired);
        targetCrewAtWork = Mathf.Clamp(targetCrewAtWork, 0, blockData.CurrentCrewHired - targetCrewAtRest);
        DistributeCrew();
        SaveBlockData();
    }

    public void SendAllCrewToIdle()
    {
        targetCrewAtWork = 0;
        targetCrewAtRest = 0;
        DistributeCrewToIdle();
        SaveBlockData();
    }

    protected void DistributeCrewToIdle()
    {
        ClearCrewLists();
        foreach (var member in crewMembers)
        {
            member.GotoIdle(GetAvailableIdlePosition());
            idleCrew.Add(member);
        }
        UpdateCrewCounts();
        OnCrewDistributed();
    }

    protected virtual Vector3 GetAvailableRestPosition()
    {
        return Vector3.zero;
    }

    protected virtual Vector3 GetAvailableIdlePosition()
    {
        if (idlePositionList.Count > idleCrew.Count)
        {
            return idlePositionList[idleCrew.Count].position;
        }
        return transform.position;
    }

    protected virtual void InitializeLists()
    {
        if (idlePositionParent != null)
        {
            foreach (Transform position in idlePositionParent)
            {
                idlePositionList.Add(position);
            }
        }

        if (workBenchesParent != null)
        {
            for (int i = 0; i < workBenchesParent.childCount; i++)
            {
                WorkBenchController workBenchController = workBenchesParent.GetChild(i).GetComponent<WorkBenchController>();
                if (workBenchController != null)
                {
                    workBenchesList.Add(workBenchController);
                }
            }
        }
    }

    protected void RestoreCrewAssignment()
    {
        ClearCrewLists();

        for (int i = 0; i < Mathf.Min(blockData.CrewAtWork, crewMembers.Count); i++)
        {
            if (i < workBenchesList.Count && workBenchesList[i] != null)
            {
                crewMembers[i].GoToWork(workBenchesList[i].GetWorkPosition());
                workingCrew.Add(crewMembers[i]);
            }
            else
            {
                if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
                {
                    crewMembers[i].GotoIdle(GetAvailableIdlePosition());
                    idleCrew.Add(crewMembers[i]);
                }
            }
        }

        int restedCount = 0;
        for (int i = 0; i < crewMembers.Count && restedCount < blockData.CrewAtRest; i++)
        {
            if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
            {
                crewMembers[i].GoToRest(GetAvailableRestPosition());
                restingCrew.Add(crewMembers[i]);
                restedCount++;
            }
        }

        foreach (var member in crewMembers)
        {
            if (workingCrew.All(c => c != member) && restingCrew.All(c => c != member) && idleCrew.All(c => c != member))
            {
                member.GotoIdle(GetAvailableIdlePosition());
                idleCrew.Add(member);
            }
        }

        UpdateCrewCounts();
        OnCrewDistributed();
    }

    public virtual void HireNewCrewMember()
    {
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.maxCrew.Value)
        {
            var newCrewMemberGO = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[(int)GetBlockType()], transform);
            CharacterController newCrewController = newCrewMemberGO.GetComponent<CharacterController>();

            if (newCrewController != null)
            {
                crewMembers.Add(newCrewController);
                allCrewMembers.Add(newCrewController);
                newCrewController.GotoIdle(GetAvailableIdlePosition());
                idleCrew.Add(newCrewController);
                UpdateCrewCounts();
                blockData.CurrentCrewHired++;
                SaveBlockData();
            }
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
    }

    public virtual void UnlockWorkBench()
    {
        if (blockData.WorkBenchesLevelUnlocked < workBenchesParent.childCount)
        {
            blockData.WorkBenchesLevelUnlocked++;

            if (blockData.WorkBenchesLevelUnlocked <= workBenchesParent.childCount)
            {
                Transform nextBench = workBenchesParent.GetChild(blockData.WorkBenchesLevelUnlocked - 1);
                WorkBenchController workBenchController = nextBench.GetComponent<WorkBenchController>();
                if (workBenchController != null)
                {
                    workBenchesList.Add(workBenchController);
                    workBenchController.gameObject.SetActive(true);
                    SaveBlockData();
                }
            }
            else
            {
                Debug.Log("Все верстаки в этом блоке уже разблокированы.");
            }
        }
        else
        {
            Debug.Log("Достигнут максимальный уровень верстаков в этом блоке.");
        }
    }

    public void UpgradeMaxCrew()
    {
        blockData.MaxCrewUnlocked ++;
        SaveBlockData();
        Debug.Log($"Максимальное количество экипажа в отделе {GetBlockType()} увеличено. Текущий лимит: {blockData.MaxCrewUnlocked}");
    }

    protected virtual void OnCrewDistributed()
    {
        // Виртуальный метод, который дочерние классы могут переопределить
    }
}