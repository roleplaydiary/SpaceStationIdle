using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CrewManager : MonoBehaviour
{
    private StationController stationController;
    private StationBlockData blockData;
    private StationBlockDataSO stationBlockDataSo;
    
    private List<CharacterController> crewMembers = new List<CharacterController>();
    public ReactiveCollection<CharacterController> workingCrew = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> restingCrew = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> idleCrew = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> allCrewMembers = new ReactiveCollection<CharacterController>();
    
    public virtual void CrewInitialization(StationBlockData blockData, StationBlockDataSO stationBlockDataSo)
    {
        stationController = ServiceLocator.Get<StationController>();
        this.blockData = blockData;
        this.stationBlockDataSo = stationBlockDataSo;
        
        if (blockData.MaxCrewUnlocked == 0 || blockData.CurrentCrewHired == 0 ||
            stationBlockDataSo.crewPrefabs == null || stationBlockDataSo.crewPrefabs.Length == 0)
        {
            Debug.LogError("Ошибка инициализации экипажа");
            return;
        }

        for (int i = 0; i < blockData.CurrentCrewHired; i++)
        {
            int prefabIndex = i % stationBlockDataSo.crewPrefabs.Length;
            var newCrewMember = SpawnNewCrewMember(prefabIndex);
        }
        
        workingCrew.ObserveCountChanged()
            .Subscribe(value =>
            {
                blockData.CrewAtWork = value;
            })
            .AddTo(this);

        restingCrew.ObserveCountChanged().Subscribe(value =>
        {
            blockData.CrewAtRest = value;
        }).AddTo(this);
    }

    private CharacterController SpawnNewCrewMember(int prefabIndex)
    {
        if (prefabIndex >= 0 && prefabIndex < stationBlockDataSo.crewPrefabs.Length)
        {
            GameObject prefabToSpawn = stationBlockDataSo.crewPrefabs[prefabIndex];
            if (prefabToSpawn != null)
            {
                var newCrewMemberGO = Instantiate(prefabToSpawn, transform);
                CharacterController newCrewController = newCrewMemberGO.GetComponent<CharacterController>();
                if (newCrewController != null)
                {
                    crewMembers.Add(newCrewController);
                    allCrewMembers.Add(newCrewController);
                    return newCrewController;
                }
            }
        }

        return null;
    }
    
    public void HireNewCrewMember(List<Transform> idlePositionList)
    {
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.MaxCrew.Value)
        {
            int prefabIndex = crewMembers.Count % stationBlockDataSo.crewPrefabs.Length;
            CharacterController newCrewController = SpawnNewCrewMember(prefabIndex);
            if (newCrewController != null)
            {
                newCrewController.GotoIdle(GetAvailableIdlePosition(newCrewController, idlePositionList));
                idleCrew.Add(newCrewController);
                blockData.CurrentCrewHired++;
            }
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
    }
    
    // Новые методы для изменения количества рабочих и отдыхающих
    public void AddCrewToWork(List<WorkBenchController> workBenchesList)
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
                    stationController.ReleaseRestPosition(workerToSend);
                }

                workerToSend.GoToWork(GetWorkPosition(workerToSend, workBenchesList));
                workingCrew.Add(workerToSend);
            }
        }
    }
    
    public void RemoveCrewFromWork( List<Transform> idlePositionList)
    {
        var workerToIdle = workingCrew.LastOrDefault();
        if (workerToIdle != null)
        {
            workingCrew.Remove(workerToIdle);
            workerToIdle.GotoIdle(GetAvailableIdlePosition(workerToIdle, idlePositionList));
            idleCrew.Add(workerToIdle);
        }
    }
    
    public void AddCrewToRest()
    {
        // Ищем первого бездействующего (idle) сотрудника
        var workerToSend = idleCrew.FirstOrDefault();
        if (workerToSend != null)
        {
            var restPosition = stationController.GetRestPosition(workerToSend);
            if (restPosition != null)
            {
                idleCrew.Remove(workerToSend);
                workerToSend.GoToRest(restPosition.position);
                restingCrew.Add(workerToSend);
            }
        }
        else
        {
            // Если в idle никого нет, берем первого работающего
            workerToSend = workingCrew.LastOrDefault();
            if (workerToSend != null)
            {
                var restPosition = stationController.GetRestPosition(workerToSend);
                if (restPosition != null)
                {
                    workingCrew.Remove(workerToSend);
                    workerToSend.GoToRest(restPosition.position);
                    restingCrew.Add(workerToSend);
                }
            }
        }
    }
    
    public void RemoveCrewFromRest(List<Transform> idlePositionList)
    {
        var workerToIdle = restingCrew.FirstOrDefault();
        if (workerToIdle != null)
        {
            restingCrew.Remove(workerToIdle);
            stationController.ReleaseRestPosition(workerToIdle);

            var idlePosition = GetAvailableIdlePosition(workerToIdle, idlePositionList);
            workerToIdle.GotoIdle(idlePosition);
            idleCrew.Add(workerToIdle);
        }
    }
    
    public void RestoreCrewAssignment(List<WorkBenchController> workBenchesList, List<Transform> idlePositionList)
    {
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
                    var idlePosition = GetAvailableIdlePosition(crewMembers[i], idlePositionList);
                    crewMembers[i].GotoIdle(idlePosition);
                    idleCrew.Add(crewMembers[i]);
                }
            }
        }

        int restedCount = 0;
        for (int i = 0; i < crewMembers.Count && restedCount < blockData.CrewAtRest; i++)
        {
            if (workingCrew.All(c => c != crewMembers[i]) && restingCrew.All(c => c != crewMembers[i]))
            {
                var restPosition = stationController.GetRestPosition(crewMembers[i]);
                if (restPosition != null)
                {
                    crewMembers[i].GoToRest(restPosition.position);
                    restingCrew.Add(crewMembers[i]);
                    restedCount++;
                }
                else
                {
                    crewMembers[i].GotoIdle(GetAvailableIdlePosition(crewMembers[i], idlePositionList));
                    idleCrew.Add(crewMembers[i]);
                }
            }
        }

        foreach (var member in crewMembers)
        {
            if (workingCrew.All(c => c != member) && restingCrew.All(c => c != member) && idleCrew.All(c => c != member))
            {
                member.GotoIdle(GetAvailableIdlePosition(member, idlePositionList));
                idleCrew.Add(member);
            }
        }
    }
    
    protected virtual Vector3 GetAvailableIdlePosition(CharacterController crewMember, List<Transform> idlePositionList)
    {
        int index = crewMembers.IndexOf(crewMember);
        if (index >= 0 && index < idlePositionList.Count)
        {
            return idlePositionList[index].position;
        }
        return transform.position; // Запасной вариант
    }

    private Vector3 GetWorkPosition(CharacterController crewMember, List<WorkBenchController> workBenchesList)
    {
        int index = crewMembers.IndexOf(crewMember);
        if (index >= 0 && index < workBenchesList.Count)
        {
            return workBenchesList[index].GetWorkPosition();
        }
        return Vector3.zero;
    }
}