using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EngineeringBlockController : StationBlockController
{
    private StationData stationData;

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

        // Подписываемся на изменение количества рабочих и пересчитываем производство
        workingCrew.ObserveCountChanged().Subscribe(_ => CalculateEnergyProduction()).AddTo(this);
        CalculateEnergyProduction(); // Первоначальный расчет при старте
    }

    private void CalculateEnergyProduction()
    {
        float totalProduction = 0f;
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            totalProduction += workBenchesList[i].ProductionRate;
        }

        // Устанавливаем значение производства энергии в DepartmentEnergyController
        if (EnergyController)
        {
            EnergyController.currentEnergyProduction.Value = totalProduction;
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
        CalculateEnergyProduction(); // Первоначальный расчет при инициализации
    }

    protected override void OnCrewDistributed()
    {
        base.OnCrewDistributed();
        CalculateEnergyProduction(); // Перерасчет после распределения экипажа
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
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.MaxCrew.Value)
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
        CalculateEnergyProduction();
    }

    protected override void InitializeLists()
    {
        base.InitializeLists();
    }
}