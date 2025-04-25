using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class BarBlockController : StationBlockController
{
    private StationData stationData;
    
    private ReactiveProperty<bool> isProductionOn = new ReactiveProperty<bool>(false);
    private CompositeDisposable disposables = new CompositeDisposable();

    [SerializeField] private Transform restPositionParent;
    private List<RestPositionController> restPositionList = new List<RestPositionController>();

    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        RestPositionInitialization();
        CalculateMoodProduction(); // Первоначальный расчет при инициализации
        
        stationData = stationController.StationData;
        if (stationData == null)
        {
            Debug.LogError("StationData не найден через StationController!");
            return;
        }
        
        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        energyService.CurrentStationEnergy
            .Subscribe(value => isProductionOn.Value = value > 0)
            .AddTo(disposables);

        // Подписываемся на изменение количества рабочих и пересчитываем производство
        crewManager.workingCrew.ObserveCountChanged()
            .Where(_ => isProductionOn.Value)
            .Subscribe(_ => CalculateMoodProduction()).AddTo(this);
    }

    private void CalculateMoodProduction()
    {
        float totalProduction = 0f;
        int workingCrewCount = crewManager.workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            totalProduction += workBenchesList[i].GetProductionRate();
        }

        if (MoodController != null)
        {
            MoodController.currentMoodEffect.Value = totalProduction;
        }
    }
    
    public override float GetProductionValue()
    {
        float result = 0f;
        int workingCrewCount = crewManager.workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            result += workBenchesList[i].GetProductionRate();
        }
        
        return result;
    }

    

    private void RestPositionInitialization()
    {
        for (int i = 0; i < restPositionList.Count; i++)
        {
            if (i < blockData.WorkStationsInstalled * 2)// TODO: Сейчас у нас просто на 1 барную стойку - 2 рест позишн. Надо перепродумать этот момент
            {
                restPositionList[i].UnlockRestPosition();
            }
        }
    }

    protected override void OnCrewDistributed()
    {
        base.OnCrewDistributed();
        CalculateMoodProduction(); // Перерасчет после распределения экипажа
    }

    public override void AddWorkBench()
    {
        base.AddWorkBench();
        // Пересчитываем производство, так как новый верстак может производить энергию
        CalculateMoodProduction();
    }

    protected override void InitializeLists()
    {
        base.InitializeLists();
        restPositionList.Clear();
        if (restPositionParent != null)
        {
            foreach (Transform child in restPositionParent)
            {
                RestPositionController restPositionController = child.GetComponent<RestPositionController>();
                if (restPositionController != null)
                {
                    restPositionList.Add(restPositionController);
                }
                else
                {
                    Debug.LogError($"Дочерний объект {child.name} в {restPositionParent.name} не имеет компонента RestPositionController!");
                }
            }
        }
    }
    
    public override Transform GetBlockRestPosition(CharacterController crewMember)
    {
        foreach (var restPosition in restPositionList)
        {
            if (restPosition.IsUnlocked)
            {
                if (!restPosition.IsOccupied)
                {
                    restPosition.OccupyRestPosition(crewMember);
                    return restPosition.transform;
                }
            }
        }
    
        return null;
    }
    
    public override void ReleaseRestPosition(CharacterController crewMember)
    {
        foreach (var restPos in restPositionList)
        {
            if (restPos.GetRestCrewMember() == crewMember)
            {
                restPos.ReleaseRestPosition();;
                return;
            }
        }
    }
}