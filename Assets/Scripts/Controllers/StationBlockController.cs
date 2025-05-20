using UnityEngine;
using System.Collections.Generic;
using Controllers;

public class StationBlockController : MonoBehaviour
{
    [SerializeField] protected Transform workBenchesParent;
    [SerializeField] protected Transform idlePositionParent;
    [SerializeField] protected StationBlockDataSO stationBlockDataSo;
    protected List<CharacterController> CrewMembers = new List<CharacterController>();
    public Department GetBlockType() { return stationBlockDataSo.BlockType; }

    protected StationBlockData blockData;

    protected List<Transform> idlePositionList = new List<Transform>();
    public List<WorkBenchController> workBenchesList = new List<WorkBenchController>();

    protected StationController StationController;
    protected DepartmentEnergyController EnergyController;
    protected DepartmentMoodController MoodController;
    protected CrewManager CrewManager;
    public CrewManager GetCrewManager() { return CrewManager; }

    public virtual void BlockInitialization(StationBlockData _blockData)
    {
        blockData = _blockData;
        StationController = ServiceLocator.Get<StationController>();

        InitializeLists();
        BenchesInitialization();

        CrewManager = gameObject.GetComponent<CrewManager>();
        
        EnergyController = gameObject.GetComponent<DepartmentEnergyController>();
        EnergyController.Initialize(this);
        
        MoodController = gameObject.GetComponent<DepartmentMoodController>();
        MoodController.Initialize(this);
    }

    public void BlockCrewInitialization()
    {
        CrewManager.CrewInitialization(blockData, stationBlockDataSo);
        RestoreCrewAssignment();
        CrewManager.ReactiveVariabliesSubscribe();
    }

    protected void BenchesInitialization()
    {
        if (blockData.WorkStationsInstalled == 0)
            return;

        Debug.Log($"StationBlockController: {name} Benches Initialization");
        workBenchesList.Clear();
        if (workBenchesParent != null)
        {
            for (int i = 0; i < blockData.WorkStationsInstalled && i < workBenchesParent.childCount; i++)
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

    private async void SaveBlockData()
    {
        if (StationController != null)
        {
            Department currentDepartment = GetBlockType();
            await ServiceLocator.Get<CloudController>().SaveDepartmentData(blockData, currentDepartment);
        }
        else
        {
            Debug.LogError("StationController не найден в ServiceLocator!");
        }
    }

    public void AddCrewToWork()
    {
        CrewManager.AddCrewToWork(workBenchesList);
        SaveBlockData();
    }

    public void RemoveCrewFromWork()
    {
        CrewManager.RemoveCrewFromWork(idlePositionList);
        SaveBlockData();
    }

    public void AddCrewToRest()
    {
       CrewManager.AddCrewToRest();
       SaveBlockData();
    }

    public void RemoveCrewFromRest()
    {
        CrewManager.RemoveCrewFromRest(idlePositionList);
        SaveBlockData();
    }

    // protected void DistributeCrewToIdle()
    // {
    //     foreach (var member in crewMembers)
    //     {
    //         member.GotoIdle(GetAvailableIdlePosition(member));
    //         idleCrew.Add(member);
    //     }
    //     OnCrewDistributed();
    // }

    protected virtual Vector3 GetAvailableIdlePosition(CharacterController crewMember)
    {
        int index = CrewMembers.IndexOf(crewMember);
        if (index >= 0 && index < idlePositionList.Count)
        {
            return idlePositionList[index].position;
        }
        return transform.position; // Запасной вариант
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
    }

    protected void RestoreCrewAssignment()
    {
        CrewManager.RestoreCrewAssignment(workBenchesList, idlePositionList);
        OnCrewDistributed();
    }

    public virtual void HireNewCrewMember()
    {
        CrewManager.HireNewCrewMember(idlePositionList);
        SaveBlockData();
    }

    public virtual void AddWorkBench()
    {
        if (blockData.WorkStationsInstalled < blockData.WorkStationsMax)
        {
            if (blockData.WorkStationsInstalled < workBenchesParent.childCount)
            {
                blockData.WorkStationsInstalled++;

                if (blockData.WorkStationsInstalled <= workBenchesParent.childCount)
                {
                    Transform nextBench = workBenchesParent.GetChild(blockData.WorkStationsInstalled - 1);
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
        blockData.WorkStationsMax++;
        SaveBlockData();
        Debug.Log($"Максимальное количество верстаков в отделе {GetBlockType()} увеличено. Текущий лимит: {blockData.WorkStationsMax}");
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
        int workingCrewCount = CrewManager.workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            result += workBenchesList[i].GetProductionRate();
        }
        
        return result;
    }

    public virtual Transform GetBlockRestPosition(CharacterController crewMember)
    {
        return null;
    }

    public virtual void ReleaseRestPosition(CharacterController crewMember)
    {
        return;
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