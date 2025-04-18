using UniRx;
using UnityEngine;

public class DepartmentEnergyController : MonoBehaviour
{
    public ReactiveProperty<float> currentEnergyProduction { get; private set; } = new ReactiveProperty<float>(0f);
    public ReactiveProperty<float> currentEnergyConsumption { get; private set; } = new ReactiveProperty<float>(0f);

    private StationBlockController blockController;

    public void Initialize(StationBlockController block)
    {
        blockController = block;
        // Подписываемся на изменение количества рабочих и пересчитываем энергию
        blockController.workingCrew.ObserveCountChanged().Subscribe(_ => CalculateEnergy()).AddTo(this);
        CalculateEnergy(); // Начальный расчет
    }

    private void CalculateEnergy()
    {
        float production = 0f;
        float consumption = 0f;
        int workingCrewCount = blockController.workingCrew.Count;
        int workBenchesCount = blockController.workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            WorkBenchController bench = blockController.workBenchesList[i];
            if (bench)
            {
                if (blockController is EngineeringBlockController && bench.ProducedResource == WorkBenchResource.Energy)
                {
                    production += bench.ProductionRate;
                }
                consumption += bench.EnergyConsumptionRate;
            }
        }

        currentEnergyProduction.Value = production;
        currentEnergyConsumption.Value = consumption;
    }
}