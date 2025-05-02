using Controllers;
using UniRx;
using UnityEngine;

namespace Services
{
    public class CrewService : MonoBehaviour
    {
        public Subject<int> OnWorkingCrewValueUpdate { get; } = new(); // int ничего не значит, просто передаём, чтобы не было ошибки
        public Subject<int> OnRestingCrewValueUpdate { get; } = new(); // int ничего не значит, просто передаём, чтобы не было ошибки

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void Initialize()
        {
            var stationController = ServiceLocator.Get<StationController>();
            foreach (var block in stationController.StationBlocks)
            {
                if (stationController.StationData.IsUnlocked(block.GetBlockType()))
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
        
            OnWorkingCrewValueUpdate.OnNext(0);
            OnRestingCrewValueUpdate.OnNext(0);
        }
    }
}
