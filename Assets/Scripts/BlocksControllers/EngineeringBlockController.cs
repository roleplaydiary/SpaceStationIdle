using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EngineeringBlockController : StationBlockController
{
    private StationData stationData;

    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        CalculateEnergyProduction(); // Первоначальный расчет при инициализации

        // Подписываемся на изменение количества рабочих и пересчитываем производство
        CrewManager.workingCrew.ObserveCountChanged().Subscribe(_ => CalculateEnergyProduction()).AddTo(this);
        CalculateEnergyProduction(); // Первоначальный расчет при старте
    }

    private void CalculateEnergyProduction()
    {
        float totalProduction = 0f;
        int workingCrewCount = CrewManager.workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            totalProduction += workBenchesList[i].GetProductionRate();
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
        int workingCrewCount = CrewManager.workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            result += workBenchesList[i].GetProductionRate();
        }
        
        return result;
    }

    

    protected override void OnCrewDistributed()
    {
        base.OnCrewDistributed();
        CalculateEnergyProduction(); // Перерасчет после распределения экипажа
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