using System;
using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EngineeringBlockController : StationBlockController
{
    private StationData stationData;
    private IDisposable energyProductionSubscription;
    private StationController stationController; // Добавляем ссылку на StationController
    
    private void Start()
    {
        stationController = ServiceLocator.Get<StationController>(); // Получаем StationController
        if (stationController == null)
        {
            Debug.LogError("StationController не найден в ServiceLocator!");
            return;
        }
        stationData = stationController.StationData; // Получаем ссылку на StationData через контроллер
        if (stationData == null)
        {
            Debug.LogError("StationData не найден через StationController!");
            return;
        }

        // Создаем реактивное выражение для отслеживания количества рабочих
        var workingCrewCountObservable = workingCrew.ObserveCountChanged().StartWith(workingCrew.Count);

        // Подписываемся на изменение количества рабочих и пересчитываем энергию
        energyProductionSubscription = workingCrewCountObservable
            .Subscribe(_ => RecalculateEnergyProduction());
    }
    
    private void OnDestroy()
    {
        energyProductionSubscription?.Dispose(); // Важно отписаться при уничтожении объекта
    }

    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        RecalculateEnergyProduction(); // Первоначальный расчет при инициализации
    }

    protected override void OnCrewDistributed()
    {
        base.OnCrewDistributed();
        RecalculateEnergyProduction(); // Перерасчет после распределения экипажа
    }

    private void RecalculateEnergyProduction()
    {
        float totalProduction = 0f;
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        if (stationData != null)
        {
            for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
            {
                if (workBenchesList[i].ProducedResource == WorkBenchResource.Energy)
                {
                    totalProduction += workBenchesList[i].ProductionRate;
                }
            }

            // Обновляем значение энергии в *том же самом* экземпляре StationData
            stationController.StationData.stationEnergy.Value = totalProduction;
        }
    }

    protected override void BenchesInitialization()
    {
        base.BenchesInitialization();
        // Нет необходимости вызывать RecalculateEnergyProduction здесь,
        // так как экипаж еще не назначен. Расчет произойдет при BlockInitialization или распределении.
    }

    protected override void CrewInitialization()
    {
        base.CrewInitialization();
        // Нет необходимости вызывать RecalculateEnergyProduction здесь,
        // так как экипаж только создается. Расчет произойдет при BlockInitialization или распределении.
    }

    private void HireNewCrewMemberInternal()
    {
        base.HireNewCrewMember();
        // RecalculateEnergyProduction вызовется через подписку на allCrewMembers
    }

    public override void HireNewCrewMember()
    {
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.maxCrew.Value)
        {
            base.HireNewCrewMember();
            // RecalculateEnergyProduction вызовется через подписку на allCrewMembers
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
    }

    public override void UnlockWorkBench()
    {
        base.UnlockWorkBench();
        // Возможно, потребуется пересчет, если мощность новых верстаков влияет на общее производство
        RecalculateEnergyProduction();
    }

    protected override Vector3 GetAvailableIdlePosition()
    {
        return base.GetAvailableIdlePosition();
    }

    protected override void InitializeLists()
    {
        base.InitializeLists();
    }
}