using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BridgeBlockController : StationBlockController
{
    public ReactiveProperty<int> crewAtWork { get; private set; }
    public ReactiveProperty<int> crewAtRest { get; private set; }
    public ReactiveProperty<int> crewAtIdle { get; private set; }

    private List<CharacterController> workingCrew = new List<CharacterController>();
    private List<CharacterController> restingCrew = new List<CharacterController>();
    private List<CharacterController> idleCrew = new List<CharacterController>();
    private List<CharacterController> crewMembers = new List<CharacterController>();
    private List<CharacterController> allCrewMembers = new List<CharacterController>();
    
    private List<Transform> idlePositionList = new List<Transform>();
    private List<WorkBenchController> workBenchesList = new List<WorkBenchController>();

    public override void BlockInitialization(StationBlockData _blockData)
    {
        crewAtWork = new ReactiveProperty<int>(0);
        crewAtRest = new ReactiveProperty<int>(0);
        crewAtIdle = new ReactiveProperty<int>(0);

        InitializeLists();

        blockData = _blockData;
        BenchesInitialization();
        CrewInitialization();
        // DistributeCrewToIdle(); // Изначально весь экипаж отправляем в Idle - теперь это делает RestoreCrewAssignment, если нужно
        RestoreCrewAssignment(); // Распределяем экипаж на основе сохраненных данных
    }

    protected override void BenchesInitialization()
    {
        if (blockData.WorkBenchesLevelUnlocked == 0)
            return;

        for (int i = 0; i < blockData.WorkBenchesLevelUnlocked; i++)
        {
            if (i < workBenchesList.Count)
                workBenchesList[i].gameObject.SetActive(true);
        }
    }

    protected override void CrewInitialization()
    {
        if (blockData.MaxCrewUnlocked == 0 || blockData.CurrentCrewHired == 0)
            return;

        for (int i = 0; i < blockData.CurrentCrewHired; i++)
        {
            var newCrewMember = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[0], transform);
            CharacterController crewController = newCrewMember.GetComponent<CharacterController>();
            crewMembers.Add(crewController);
            allCrewMembers.Add(crewController);
        }
        RestoreCrewAssignment(); // Распределяем экипаж после создания
    }
    
    private void HireNewCrewMemberInternal()
    {
        var newCrewMember = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[0], transform);
        CharacterController crewController = newCrewMember.GetComponent<CharacterController>();
        if (crewController != null)
        {
            crewMembers.Add(crewController);
            allCrewMembers.Add(crewController); // Добавляем в список всех членов экипажа
        }
    }
    
    // Метод, вызываемый из StationController при найме
    public override void HireNewCrewMember()
    {
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.maxCrew.Value)
        {
            HireNewCrewMemberInternal();
            DistributeCrew(); // Перераспределяем экипаж после найма
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
    }

    private void ClearCrewLists()
    {
        workingCrew.Clear();
        restingCrew.Clear();
        idleCrew.Clear();
    }

    private void UpdateCrewCounts()
    {
        crewAtWork.Value = workingCrew.Count;
        crewAtRest.Value = restingCrew.Count;
        crewAtIdle.Value = idleCrew.Count;
    }

    private void DistributeCrew()
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
        blockData.CrewAtWork = workingCrew.Count;

        // Распределяем отдыхающих (начиная с тех, кто не работает)
        int restedCount = 0;
        for (int i = 0; i < crewMembers.Count && restedCount < targetCrewAtRest; i++)
        {
            if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
            {
                crewMembers[i].GoToRest(GetAvailableRestPosition()); // Вам понадобится логика для определения места отдыха
                restingCrew.Add(crewMembers[i]);
                restedCount++;
            }
        }
        blockData.CrewAtRest = restingCrew.Count;

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

    private int targetCrewAtWork = 0;
    private int targetCrewAtRest = 0;

    public void SendCrewToWork(int crewCount)
    {
        targetCrewAtWork = Mathf.Clamp(crewCount, 0, blockData.CurrentCrewHired);
        targetCrewAtRest = Mathf.Clamp(targetCrewAtRest, 0, blockData.CurrentCrewHired - targetCrewAtWork);
        DistributeCrew();
    }

    public void SendCrewToRest(int crewCount)
    {
        targetCrewAtRest = Mathf.Clamp(crewCount, 0, blockData.CurrentCrewHired);
        targetCrewAtWork = Mathf.Clamp(targetCrewAtWork, 0, blockData.CurrentCrewHired - targetCrewAtRest);
        DistributeCrew();
    }

    public void SendAllCrewToIdle()
    {
        targetCrewAtWork = 0;
        targetCrewAtRest = 0;
        DistributeCrewToIdle();
    }

    private void DistributeCrewToIdle()
    {
        ClearCrewLists();
        foreach (var member in crewMembers)
        {
            member.GotoIdle(GetAvailableIdlePosition());
            idleCrew.Add(member);
        }
        UpdateCrewCounts();
    }

    // Вспомогательные функции для получения доступных позиций (реализуйте их)
    private Vector3 GetAvailableWorkPosition(CharacterController crewMember)
    {
        // Логика поиска свободной рабочей станции
        return Vector3.zero;
    }

    private Vector3 GetAvailableRestPosition()
    {
        // Логика поиска свободной зоны отдыха
        return Vector3.zero;
    }

    private Vector3 GetAvailableIdlePosition()
    {
        if (idlePositionList.Count > idleCrew.Count)
        {
            return idlePositionList[idleCrew.Count].position;
        }
        return transform.position; // В качестве запасного варианта
    }
    
    private void RestoreCrewAssignment()
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
    
    private void InitializeLists()
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
}