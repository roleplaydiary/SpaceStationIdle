using UniRx;
using UnityEngine;

public class DepartmentEnergyController : MonoBehaviour, IDepartmentEnergyUser
{
    public ReactiveProperty<float> currentEnergyProduction { get; private set; } = new ReactiveProperty<float>(0f);
    public ReactiveProperty<float> currentEnergyConsumption { get; private set; } = new ReactiveProperty<float>(0f);

    public IReadOnlyReactiveProperty<float> NetEnergyChange => _netEnergyChange;
    private ReactiveProperty<float> _netEnergyChange = new ReactiveProperty<float>(0f);

    private StationBlockController blockController;
    private CompositeDisposable disposables = new CompositeDisposable();

    public void Initialize(StationBlockController block)
    {
        blockController = block;

        // Подписываемся на изменение количества рабочих
        blockController.GetCrewManager().workingCrew.ObserveCountChanged().Subscribe(_ => RecalculateEnergy()).AddTo(disposables);

        // Начальный расчет энергии
        RecalculateEnergy();

        // Регистрация в StationEnergyService
        var energyService = ServiceLocator.Get<StationEnergyService>();
        energyService.RegisterEnergyUser(this);
    }

    private void OnDestroy()
    {
        var energyService = ServiceLocator.Get<StationEnergyService>();
        energyService?.UnregisterEnergyUser(this);
        disposables.Clear();
    }

    private void RecalculateEnergy()
    {
        float production = 0f;
        float consumption = 0f;
        int workingCrewCount = blockController.GetCrewManager().workingCrew.Count;
        int workBenchesCount = blockController.workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            WorkBenchController bench = blockController.workBenchesList[i];
            if (blockController is EngineeringBlockController)
            {
                production += bench.ProductionRate;
            }
            consumption += bench.EnergyConsumptionRate;
        }

        currentEnergyProduction.Value = production;
        currentEnergyConsumption.Value = consumption;
        _netEnergyChange.Value = production - consumption;
    }
}