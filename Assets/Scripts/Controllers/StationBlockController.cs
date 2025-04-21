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
    public ReactiveCollection<CharacterController> restingCrew = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> idleCrew = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> allCrewMembers = new ReactiveCollection<CharacterController>();

    protected List<Transform> idlePositionList = new List<Transform>();
    public List<WorkBenchController> workBenchesList = new List<WorkBenchController>();

    protected int targetCrewAtWork = 0;
    protected int targetCrewAtRest = 0;

    protected StationController stationController;
    protected DepartmentEnergyController EnergyController;
    protected DepartmentMoodController MoodController;

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
        
        MoodController = gameObject.GetComponent<DepartmentMoodController>();
        MoodController.Initialize(this);

        crewAtWork.Subscribe(_crewAtWork =>
        {
            blockData.CrewAtWork = _crewAtWork;
        }).AddTo(this);

        crewAtRest.Subscribe(_crewAtRest =>
        {
            blockData.CrewAtRest = _crewAtRest;
        }).AddTo(this);

        UpdateCrewCounts(); // Добавлено для начального отображения
    }

    protected virtual void BenchesInitialization()
    {
        if (blockData.WorkBenchesInstalled == 0)
            return;

        if (workBenchesParent != null)
        {
            for (int i = 0; i < blockData.WorkBenchesInstalled && i < workBenchesParent.childCount; i++)
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

    // Новые методы для изменения количества рабочих и отдыхающих
    public void AddCrewToWork()
    {
        if (idleCrew.Count > 0)
        {
            var crewMember = idleCrew.First();
            idleCrew.Remove(crewMember);
            crewMember.GoToWork(GetAvailableWorkPosition());
            workingCrew.Add(crewMember);
            UpdateCrewCounts();
            SaveBlockData();
        }
        else if (restingCrew.Count > 0)
        {
            var crewMember = restingCrew.First();
            restingCrew.Remove(crewMember);
            crewMember.GoToWork(GetAvailableWorkPosition());
            workingCrew.Add(crewMember);
            UpdateCrewCounts();
            SaveBlockData();
        }
    }

    public void RemoveCrewFromWork()
    {
        if (workingCrew.Count > 0)
        {
            var crewMember = workingCrew.Last();
            workingCrew.Remove(crewMember);
            crewMember.GotoIdle(GetAvailableIdlePosition());
            idleCrew.Add(crewMember);
            UpdateCrewCounts();
            SaveBlockData();
        }
    }

    public void AddCrewToRest()
    {
        if (idleCrew.Count > 0)
        {
            var crewMember = idleCrew.First();
            idleCrew.Remove(crewMember);
            crewMember.GoToRest(GetAvailableRestPosition());
            restingCrew.Add(crewMember);
            UpdateCrewCounts();
            SaveBlockData();
        }
        else if (workingCrew.Count > 0)
        {
            var crewMember = workingCrew.Last();
            workingCrew.Remove(crewMember);
            crewMember.GoToRest(GetAvailableRestPosition());
            restingCrew.Add(crewMember);
            UpdateCrewCounts();
            SaveBlockData();
        }
    }

    public void RemoveCrewFromRest()
    {
        if (restingCrew.Count > 0)
        {
            var crewMember = restingCrew.Last();
            restingCrew.Remove(crewMember);
            crewMember.GotoIdle(GetAvailableIdlePosition());
            idleCrew.Add(crewMember);
            UpdateCrewCounts();
            SaveBlockData();
        }
    }

    public void SendCrewToIdle(int count)
    {
        int moved = 0;
        // Перемещаем из рабочих в idle
        for (int i = workingCrew.Count - 1; i >= 0 && moved < count; i--)
        {
            var crewMember = workingCrew[i];
            workingCrew.RemoveAt(i);
            crewMember.GotoIdle(GetAvailableIdlePosition());
            idleCrew.Add(crewMember);
            moved++;
        }

        // Если еще нужно переместить, перемещаем из отдыхающих в idle
        for (int i = restingCrew.Count - 1; i >= 0 && moved < count; i--)
        {
            var crewMember = restingCrew[i];
            restingCrew.RemoveAt(i);
            crewMember.GotoIdle(GetAvailableIdlePosition());
            idleCrew.Add(crewMember);
            moved++;
        }

        UpdateCrewCounts();
        OnCrewDistributed();
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

    protected virtual Vector3 GetAvailableWorkPosition()
    {
        if (workBenchesList.Count > workingCrew.Count && workBenchesList[workingCrew.Count] != null)
        {
            return workBenchesList[workingCrew.Count].GetWorkPosition();
        }
        return transform.position; // Или другая логика, если нет свободных верстаков
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

    public virtual void AddWorkBench()
    {
        if (blockData.WorkBenchesInstalled < blockData.WorkBenchesMax)
        {
            if (blockData.WorkBenchesInstalled < workBenchesParent.childCount)
            {
                blockData.WorkBenchesInstalled++;

                if (blockData.WorkBenchesInstalled <= workBenchesParent.childCount)
                {
                    Transform nextBench = workBenchesParent.GetChild(blockData.WorkBenchesInstalled - 1);
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
        else
        {
            Debug.Log("Достигнут максимум верстаков в отделе");
        }
    }

    public virtual void UpgradeWorkBenchMax()
    {
        blockData.WorkBenchesMax++;
        SaveBlockData();
        Debug.Log($"Максимальное количество верстаков в отделе {GetBlockType()} увеличено. Текущий лимит: {blockData.WorkBenchesMax}");
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

    public virtual float GetProductionValue()
    {
        float result = 0f;
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            result += workBenchesList[i].ProductionRate;
        }
        
        return result;
    }
    
    /// <summary>
    /// Виртуальный метод для начисления ресурсов, произведенных за время отсутствия игрока.
    /// Должен быть переопределен в дочерних классах, которые занимаются производством.
    /// </summary>
    /// <param name="afkTime">Время отсутствия игрока.</param>
    public virtual void AddAFKProduction(System.TimeSpan afkTime)
    {
        // По умолчанию ничего не производим.
    }
}