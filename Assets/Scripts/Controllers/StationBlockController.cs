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
        if (blockData.MaxCrewUnlocked == 0 || blockData.CurrentCrewHired == 0 ||
            stationBlockDataSo.crewPrefabs == null || stationBlockDataSo.crewPrefabs.Length == 0)
        {
            Debug.LogError("Ошибка инициализации экипажа");
            return;
        }

        for (int i = 0; i < blockData.CurrentCrewHired; i++)
        {
            // Определяем индекс префаба для спавна по порядку
            int prefabIndex = i % stationBlockDataSo.crewPrefabs.Length;
            GameObject prefabToSpawn = stationBlockDataSo.crewPrefabs[prefabIndex];

            if (prefabToSpawn != null)
            {
                var newCrewMember = Instantiate(prefabToSpawn, transform);
                CharacterController crewController = newCrewMember.GetComponent<CharacterController>();
                if (crewController != null)
                {
                    crewMembers.Add(crewController);
                    allCrewMembers.Add(crewController);
                }
                else
                {
                    Debug.LogError($"Префаб экипажа {prefabToSpawn.name} не имеет компонента CharacterController!");
                    Destroy(newCrewMember); // Уничтожаем проблемный объект
                }
            }
            else
            {
                Debug.LogError($"Префаб экипажа с индексом {prefabIndex} в StationBlockDataSO для {GetBlockType()} - null!");
            }
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

        int workBenchIndex = 0;
        for (int i = 0; i < Mathf.Min(targetCrewAtWork, crewMembers.Count); i++)
        {
            if (workBenchIndex < workBenchesList.Count && workBenchesList[workBenchIndex] != null)
            {
                crewMembers[i].GoToWork(workBenchesList[workBenchIndex].GetWorkPosition());
                workingCrew.Add(crewMembers[i]);
                workBenchIndex++;
            }
            else if (workingCrew.Count < targetCrewAtWork)
            {
                crewMembers[i].GotoIdle(GetAvailableIdlePosition(crewMembers[i]));
                idleCrew.Add(crewMembers[i]);
            }
        }

        int restedCount = 0;
        for (int i = 0; i < crewMembers.Count && restedCount < targetCrewAtRest; i++)
        {
            if (!workingCrew.Contains(crewMembers[i]) && !restingCrew.Contains(crewMembers[i]))
            {
                crewMembers[i].GoToRest(GetAvailableRestPosition(crewMembers[i]));
                restingCrew.Add(crewMembers[i]);
                restedCount++;
            }
        }

        foreach (var member in crewMembers)
        {
            if (!workingCrew.Contains(member) && !restingCrew.Contains(member) && !idleCrew.Contains(member))
            {
                member.GotoIdle(GetAvailableIdlePosition(member));
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
        int currentWorkers = workingCrew.Count;

        if (currentWorkers < blockData.WorkBenchesInstalled && currentWorkers < crewMembers.Count)
        {
            var workerToSend = crewMembers[currentWorkers];

            if (idleCrew.Contains(workerToSend) || restingCrew.Contains(workerToSend))
            {
                if (idleCrew.Contains(workerToSend))
                {
                    idleCrew.Remove(workerToSend);
                }
                else if (restingCrew.Contains(workerToSend))
                {
                    restingCrew.Remove(workerToSend);
                }

                workerToSend.GoToWork(workBenchesList[currentWorkers].GetWorkPosition());
                workingCrew.Add(workerToSend);
                UpdateCrewCounts();
                SaveBlockData();
            }
        }
    }

    public void RemoveCrewFromWork()
    {
        var workerToIdle = workingCrew.FirstOrDefault();
        if (workerToIdle != null)
        {
            workingCrew.Remove(workerToIdle);
            workerToIdle.GotoIdle(GetAvailableIdlePosition(workerToIdle));
            idleCrew.Add(workerToIdle);
            UpdateCrewCounts();
            SaveBlockData();
        }
    }

    public void AddCrewToRest()
    {
        // Ищем первого бездействующего (idle) сотрудника
        var workerToSend = idleCrew.FirstOrDefault();
        if (workerToSend != null)
        {
            idleCrew.Remove(workerToSend);
            workerToSend.GoToRest(GetAvailableRestPosition(workerToSend)); // Используйте персональную позицию отдыха, если она есть
            restingCrew.Add(workerToSend);
            UpdateCrewCounts();
            SaveBlockData();
        }
        else
        {
            // Если в idle никого нет, берем первого работающего
            workerToSend = workingCrew.FirstOrDefault();
            if (workerToSend != null)
            {
                workingCrew.Remove(workerToSend);
                workerToSend.GoToRest(GetAvailableRestPosition(workerToSend)); // Используйте персональную позицию отдыха, если она есть
                restingCrew.Add(workerToSend);
                UpdateCrewCounts();
                SaveBlockData();
            }
        }
    }

    public void RemoveCrewFromRest()
    {
        var workerToIdle = restingCrew.FirstOrDefault();
        if (workerToIdle != null)
        {
            restingCrew.Remove(workerToIdle);
            workerToIdle.GotoIdle(GetAvailableIdlePosition(workerToIdle));
            idleCrew.Add(workerToIdle);
            UpdateCrewCounts();
            SaveBlockData();
        }
    }

    protected void DistributeCrewToIdle()
    {
        ClearCrewLists();
        foreach (var member in crewMembers)
        {
            member.GotoIdle(GetAvailableIdlePosition(member));
            idleCrew.Add(member);
        }
        UpdateCrewCounts();
        OnCrewDistributed();
    }

    protected virtual Vector3 GetAvailableRestPosition(CharacterController crewMember)
    {
        return Vector3.zero;
    }

    protected virtual Vector3 GetAvailableWorkPosition(CharacterController crewMember)
    {
        int index = crewMembers.IndexOf(crewMember);
        if (index >= 0 && index < workBenchesList.Count && workBenchesList[index] != null)
        {
            if (blockData.WorkBenchesInstalled >= index)
            {
                return workBenchesList[index].GetWorkPosition();
            }
        }
        return transform.position; // Запасной вариант
    }

    protected virtual Vector3 GetAvailableIdlePosition(CharacterController crewMember)
    {
        int index = crewMembers.IndexOf(crewMember);
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
                    crewMembers[i].GotoIdle(GetAvailableIdlePosition(crewMembers[i]));
                    idleCrew.Add(crewMembers[i]);
                }
            }
        }

        int restedCount = 0;
        for (int i = 0; i < crewMembers.Count && restedCount < blockData.CrewAtRest; i++)
        {
            if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
            {
                crewMembers[i].GoToRest(GetAvailableRestPosition(crewMembers[i]));
                restingCrew.Add(crewMembers[i]);
                restedCount++;
            }
        }

        foreach (var member in crewMembers)
        {
            if (workingCrew.All(c => c != member) && restingCrew.All(c => c != member) && idleCrew.All(c => c != member))
            {
                member.GotoIdle(GetAvailableIdlePosition(member));
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
                newCrewController.GotoIdle(GetAvailableIdlePosition(newCrewController));
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