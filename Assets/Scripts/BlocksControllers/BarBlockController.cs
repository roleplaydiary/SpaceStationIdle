using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class RestPosition
{
    public Transform position;
    public bool IsOccupied;
    
    public RestPosition(Transform pos)
    {
        position = pos;
        IsOccupied = false;
    }
}
public class BarBlockController : StationBlockController
{
    private StationData stationData;
    
    private ReactiveProperty<bool> isProductionOn = new ReactiveProperty<bool>(false);
    private CompositeDisposable disposables = new CompositeDisposable();

    [SerializeField] private Transform restPositionParent;
    private List<RestPosition> restPositionList = new List<RestPosition>();

    
    private void Start()
    {
        stationController = ServiceLocator.Get<StationController>();
        if (stationController == null)
        {
            Debug.LogError("StationController не найден в ServiceLocator!");
            return;
        }
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
        workingCrew.ObserveCountChanged()
            .Where(_ => isProductionOn.Value)
            .Subscribe(_ => CalculateMoodProduction()).AddTo(this);
    }

    private void CalculateMoodProduction()
    {
        float totalProduction = 0f;
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            totalProduction += workBenchesList[i].ProductionRate;
        }

        if (MoodController != null)
        {
            MoodController.currentMoodEffect.Value = totalProduction;
        }
    }
    
    public override float GetProductionValue()
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

    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        CalculateMoodProduction(); // Первоначальный расчет при инициализации
    }

    protected override void OnCrewDistributed()
    {
        base.OnCrewDistributed();
        CalculateMoodProduction(); // Перерасчет после распределения экипажа
    }

    protected override void BenchesInitialization()
    {
        base.BenchesInitialization();
        // Расчет произойдет при BlockInitialization или распределении, когда экипаж будет назначен.
    }

    protected override void CrewInitialization()
    {
        base.CrewInitialization();
        // Расчет произойдет при BlockInitialization или распределении.
    }

    public override void HireNewCrewMember()
    {
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.maxCrew.Value)
        {
            base.HireNewCrewMember();
            // CalculateEnergyProduction вызовется через подписку на workingCrew
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
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
            foreach (Transform position in restPositionParent)
            {
                restPositionList.Add(new RestPosition(position));
            }
        }
    }
    
    public override Transform GetBlockRestPosition()
    {
        foreach (var restPosition in restPositionList)
        {
            if (!restPosition.IsOccupied)
            {
                restPosition.IsOccupied = true;
                return restPosition.position;
            }
        }
    
        return null;
    }
    
    public override void ReleaseRestPosition(Transform positionToRelease)
    {
        foreach (var restPos in restPositionList)
        {
            if (restPos.position.position == positionToRelease.position)
            {
                restPos.IsOccupied = false;
                return;
            }
        }
        Debug.LogWarning($"Попытка освободить несуществующую позицию отдыха: {positionToRelease}");
    }
}