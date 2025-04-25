using System;
using UniRx;
using UnityEngine;

public class CrewService : MonoBehaviour
{
    public Subject<int> OnWorkingCrewValueUpdate { get; private set; } = new Subject<int>(); // int ничего не значит, просто передаём, чтобы не было ошибки
    public Subject<int> OnRestingCrewValueUpdate { get; private set; } = new Subject<int>(); // int ничего не значит, просто передаём, чтобы не было ошибки

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void Initialize()
    {
        var stationController = ServiceLocator.Get<StationController>();
        foreach (var block in stationController.StationBlocks)
        {
            block.GetCrewManager().workingCrew.ObserveCountChanged().Subscribe(crewAtWork =>
            {
                OnWorkingCrewValueUpdate.OnNext(crewAtWork);
            }).AddTo(this);
            
            block.GetCrewManager().restingCrew.ObserveCountChanged().Subscribe(crewAtRest =>
            {
                OnRestingCrewValueUpdate.OnNext(crewAtRest);
            }).AddTo(this);
        }
    }
}
