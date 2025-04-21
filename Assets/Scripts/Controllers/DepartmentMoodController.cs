using UniRx;
using UnityEngine;

public class DepartmentMoodController : MonoBehaviour, IDepartmentMoodUser
{
    public ReactiveProperty<float> currentMoodEffect { get; private set; } = new ReactiveProperty<float>(0f);

    public IReadOnlyReactiveProperty<float> NetMoodChange => _netMoodChange;
    private ReactiveProperty<float> _netMoodChange = new ReactiveProperty<float>(0f);

    private StationBlockController blockController;
    private CompositeDisposable disposables = new CompositeDisposable();

    public void Initialize(StationBlockController block)
    {
        blockController = block;

        // Подписываемся на изменение количества рабочих
        blockController.workingCrew.ObserveCountChanged().Subscribe(_ => RecalculateMood()).AddTo(disposables);

        // Начальный расчет настроения
        RecalculateMood();

        // Регистрация в StationMoodService
        var moodService = ServiceLocator.Get<StationMoodService>();
        moodService.RegisterMoodUser(this);
    }

    private void OnDestroy()
    {
        var moodService = ServiceLocator.Get<StationMoodService>();
        moodService?.UnregisterMoodUser(this);
        disposables.Clear();
    }

    private void RecalculateMood()
    {
        float totalMoodChange = 0f;
        int workingCrewCount = blockController.workingCrew.Count;
        int workBenchesCount = blockController.workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            WorkBenchController bench = blockController.workBenchesList[i];
            // Учитываем потребление настроения с каждого рабочего места
            totalMoodChange -= bench.MoodConsumptionRate;
            if (blockController is BarBlockController)
            {
                totalMoodChange += blockController.workBenchesList[i].ProductionRate; // Значение повышения настроения в баре
            }
        }

        currentMoodEffect.Value = totalMoodChange;
        _netMoodChange.Value = totalMoodChange;
    }
}