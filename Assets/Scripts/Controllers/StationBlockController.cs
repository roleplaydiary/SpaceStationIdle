using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StationBlockController : MonoBehaviour
{
    [SerializeField] protected Transform workBenchesParent;
    [SerializeField] protected Transform idlePositionParent;
    [SerializeField] protected StationBlockDataSO stationBlockDataSo;
    protected List<CharacterController> crewMembers = new List<CharacterController>(); //Персонал в отделе
    public Department GetBlockType() { return stationBlockDataSo.BlockType; }

    protected StationBlockData blockData;

    public ReactiveProperty<int> crewAtWork { get; protected set; }
    public ReactiveProperty<int> crewAtRest { get; protected set; }
    public ReactiveProperty<int> crewAtIdle { get; protected set; }

    protected List<CharacterController> workingCrew = new List<CharacterController>();
    protected List<CharacterController> restingCrew = new List<CharacterController>();
    protected List<CharacterController> idleCrew = new List<CharacterController>();
    protected List<CharacterController> allCrewMembers = new List<CharacterController>();

    protected List<Transform> idlePositionList = new List<Transform>();
    protected List<WorkBenchController> workBenchesList = new List<WorkBenchController>();

    protected int targetCrewAtWork = 0;
    protected int targetCrewAtRest = 0;

    public virtual void BlockInitialization(StationBlockData _blockData)
    {
        crewAtWork = new ReactiveProperty<int>(0);
        crewAtRest = new ReactiveProperty<int>(0);
        crewAtIdle = new ReactiveProperty<int>(0);

        blockData = _blockData;
        
        InitializeLists();
        BenchesInitialization();
        CrewInitialization();
        RestoreCrewAssignment();
        
        crewAtWork.Subscribe(_crewAtWork =>
        {
            blockData.CrewAtWork = _crewAtWork;
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
            // Дочерние классы должны определить, как создавать членов экипажа
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

        // Распределяем работающих
        for (int i = 0; i < Mathf.Min(targetCrewAtWork, crewMembers.Count); i++)
        {
            if (i < workBenchesList.Count && workBenchesList[i] != null)
            {
                crewMembers[i].GoToWork(workBenchesList[i].GetWorkPosition());
                workingCrew.Add(crewMembers[i]);
            }
            else if (workingCrew.Count < targetCrewAtWork) // Если не хватает верстаков, остальных в Idle
            {
                if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
                {
                    crewMembers[i].GotoIdle(GetAvailableIdlePosition());
                    idleCrew.Add(crewMembers[i]);
                }
            }
        }

        // Распределяем отдыхающих (начиная с тех, кто не работает)
        int restedCount = 0;
        for (int i = 0; i < crewMembers.Count && restedCount < targetCrewAtRest; i++)
        {
            if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
            {
                crewMembers[i].GoToRest(GetAvailableRestPosition()); // Дочерние классы могут переопределить
                restingCrew.Add(crewMembers[i]);
                restedCount++;
            }
        }

        // Оставшихся отправляем в Idle
        foreach (var member in crewMembers)
        {
            if (workingCrew.All(c => c != member) && restingCrew.All(c => c != member) && idleCrew.All(c => c != member))
            {
                member.GotoIdle(GetAvailableIdlePosition()); // Дочерние классы могут переопределить
                idleCrew.Add(member);
            }
        }

        UpdateCrewCounts();
    }

    private async void SaveBlockData()
    {
        StationController stationController = ServiceLocator.Get<StationController>();
        if (stationController != null)
        {
            // Получаем тип департамента текущего блока
            Department currentDepartment = GetBlockType();
            // Сохраняем данные департамента
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
            member.GotoIdle(GetAvailableIdlePosition()); // Дочерние классы могут переопределить
            idleCrew.Add(member);
        }
        UpdateCrewCounts();
    }

    protected virtual Vector3 GetAvailableRestPosition()
    {
        // Логика поиска свободной зоны отдыха (может быть переопределена)
        return Vector3.zero;
    }

    protected virtual Vector3 GetAvailableIdlePosition()
    {
        if (idlePositionList.Count > idleCrew.Count)
        {
            return idlePositionList[idleCrew.Count].position;
        }
        return transform.position; // В качестве запасного варианта
    }

    protected virtual void InitializeLists()
    {
        // Получаем Transform для позиций Idle
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

        // Отправляем на работу столько членов экипажа, сколько было сохранено
        for (int i = 0; i < Mathf.Min(blockData.CrewAtWork, crewMembers.Count); i++)
        {
            if (i < workBenchesList.Count && workBenchesList[i] != null)
            {
                crewMembers[i].GoToWork(workBenchesList[i].GetWorkPosition());
                workingCrew.Add(crewMembers[i]);
            }
            else // Если рабочих мест меньше, чем сохраненное количество рабочих, отправляем в Idle
            {
                if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
                {
                    crewMembers[i].GotoIdle(GetAvailableIdlePosition());
                    idleCrew.Add(crewMembers[i]);
                }
            }
        }

        // Отправляем отдыхать столько членов экипажа, сколько было сохранено (из тех, кто еще не работает)
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

        // Оставшихся отправляем в Idle
        foreach (var member in crewMembers)
        {
            if (workingCrew.All(c => c != member) && restingCrew.All(c => c != member) && idleCrew.All(c => c != member))
            {
                member.GotoIdle(GetAvailableIdlePosition());
                idleCrew.Add(member);
            }
        }

        UpdateCrewCounts();
    }

    public virtual void HireNewCrewMember()
    {
        // Базовая реализация, дочерние классы могут переопределить
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.maxCrew.Value)
        {
            // Создаем нового члена экипажа
            var newCrewMemberGO = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[(int)GetBlockType()], transform);
            CharacterController newCrewController = newCrewMemberGO.GetComponent<CharacterController>();

            if (newCrewController != null)
            {
                // Добавляем его в список всех членов экипажа
                crewMembers.Add(newCrewController);
                allCrewMembers.Add(newCrewController);

                // Отправляем нового члена экипажа в Idle
                newCrewController.GotoIdle(GetAvailableIdlePosition());
                idleCrew.Add(newCrewController);
                UpdateCrewCounts(); // Обновляем счетчики экипажа

                // Увеличиваем счетчик нанятых в данных блока
                blockData.CurrentCrewHired++;

                // Сохраняем изменения данных блока
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
}