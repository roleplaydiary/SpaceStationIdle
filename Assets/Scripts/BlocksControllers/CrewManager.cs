using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CrewManager : MonoBehaviour
{
    public ReactiveProperty<int> CrewAtWork { get; private set; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> CrewAtRest { get; private set; } = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> CrewAtIdle { get; private set; } = new ReactiveProperty<int>(0);

    public ReactiveCollection<CharacterController> WorkingCrew { get; private set; } = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> RestingCrew { get; private set; } = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> IdleCrew { get; private set; } = new ReactiveCollection<CharacterController>();
    public ReactiveCollection<CharacterController> AllCrewMembers { get; private set; } = new ReactiveCollection<CharacterController>();

    private List<Transform> _idlePositions;
    private List<WorkBenchController> _workBenches;
    private Transform _departmentTransform;

    public void Initialize(List<Transform> idlePositions, List<WorkBenchController> workBenches, Transform departmentTransform)
    {
        _idlePositions = idlePositions;
        _workBenches = workBenches;
        _departmentTransform = departmentTransform;
        UpdateCrewCounts();
    }

    // public void HireNewCrewMember(Department department)
    // {
    //     var newCrewMemberGO = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[(int)department], _departmentTransform);
    //     CharacterController newCrewController = newCrewMemberGO.GetComponent<CharacterController>();
    //     if (newCrewController != null)
    //     {
    //         AllCrewMembers.Add(newCrewController);
    //         SendToIdle(newCrewController);
    //     }
    // }

    public void AddCrewToWork()
    {
        if (IdleCrew.Count > 0) MoveToWorking(IdleCrew.First());
        else if (RestingCrew.Count > 0) MoveToWorking(RestingCrew.First());
    }

    public void RemoveCrewFromWork()
    {
        if (WorkingCrew.Count > 0) SendToIdle(WorkingCrew.Last());
    }

    public void AddCrewToRest()
    {
        if (IdleCrew.Count > 0) MoveToResting(IdleCrew.First());
        else if (WorkingCrew.Count > 0) MoveToResting(WorkingCrew.Last());
    }

    public void RemoveCrewFromRest()
    {
        if (RestingCrew.Count > 0) SendToIdle(RestingCrew.Last());
    }

    public void SendAllToIdle()
    {
        foreach (var member in WorkingCrew.ToList()) SendToIdle(member);
        foreach (var member in RestingCrew.ToList()) SendToIdle(member);
    }

    public void SendToIdle(CharacterController crewMember)
    {
        if (WorkingCrew.Remove(crewMember))
        {
            crewMember.GotoIdle(GetAvailableIdlePosition());
            IdleCrew.Add(crewMember);
            UpdateCrewCounts();
        }
        else if (RestingCrew.Remove(crewMember))
        {
            crewMember.GotoIdle(GetAvailableIdlePosition());
            IdleCrew.Add(crewMember);
            UpdateCrewCounts();
        }
    }

    private void MoveToWorking(CharacterController crewMember)
    {
        if (crewMember == null) return;
        WorkingCrew.Add(crewMember);
        IdleCrew.Remove(crewMember);
        RestingCrew.Remove(crewMember);
        crewMember.GoToWork(GetAvailableWorkPosition());
        UpdateCrewCounts();
    }

    private void MoveToResting(CharacterController crewMember)
    {
        if (crewMember == null) return;
        RestingCrew.Add(crewMember);
        IdleCrew.Remove(crewMember);
        WorkingCrew.Remove(crewMember);
        crewMember.GoToRest(GetAvailableRestPosition());
        UpdateCrewCounts();
    }

    public Vector3 GetAvailableRestPosition() => Vector3.zero; // Реализуй логику получения позиции отдыха

    public Vector3 GetAvailableIdlePosition()
    {
        if (_idlePositions != null && IdleCrew.Count < _idlePositions.Count)
        {
            return _idlePositions[IdleCrew.Count].position;
        }
        return _departmentTransform.position;
    }

    public Vector3 GetAvailableWorkPosition()
    {
        if (_workBenches != null && WorkingCrew.Count < _workBenches.Count)
        {
            return _workBenches[WorkingCrew.Count].GetWorkPosition();
        }
        return _departmentTransform.position; // Или другая логика, если нет свободных верстаков
    }

    public void RestoreCrewAssignment(int workCrewCount, int restCrewCount)
    {
        SendAllToIdle();
        var availableCrew = AllCrewMembers.ToList();
        for (int i = 0; i < Mathf.Min(workCrewCount, availableCrew.Count); i++)
        {
            MoveToWorking(availableCrew[i]);
        }
        for (int i = workCrewCount; i < workCrewCount + Mathf.Min(restCrewCount, availableCrew.Count - workCrewCount); i++)
        {
            MoveToResting(availableCrew[i]);
        }
    }

    public void UpdateWorkBenches(List<WorkBenchController> workBenches)
    {
        _workBenches = workBenches;
    }

    private void UpdateCrewCounts()
    {
        CrewAtWork.Value = WorkingCrew.Count;
        CrewAtRest.Value = RestingCrew.Count;
        CrewAtIdle.Value = IdleCrew.Count;
    }
}