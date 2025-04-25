using UniRx;
using UnityEngine;

public class DepartmentMoodController : MonoBehaviour, IDepartmentMoodUser
{
    public ReactiveProperty<float> currentMoodEffect { get; private set; } = new ReactiveProperty<float>(0f);

    public IReadOnlyReactiveProperty<float> NetMoodChange => _netMoodChange;
    private ReactiveProperty<float> _netMoodChange = new ReactiveProperty<float>(0f);

    private StationBlockController blockController;
    private CompositeDisposable disposables = new CompositeDisposable();
    
    public const int rest_mood_const = 2;

    public void Initialize(StationBlockController block)
    {
        blockController = block;
        var crewManager = blockController.GetCrewManager();

        // Объединяем потоки изменений workingCrew и restingCrew
        Observable.Merge(crewManager.workingCrew.ObserveCountChanged().AsUnitObservable(),
                crewManager.restingCrew.ObserveCountChanged().AsUnitObservable())
            .Subscribe(_ => RecalculateMood())
            .AddTo(disposables);

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
        int workingCrewCount = blockController.GetCrewManager().workingCrew.Count;
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

        int restingCrewCount = blockController.GetCrewManager().restingCrew.Count;
        for (int i = 0; i < restingCrewCount; i++)
        {
            totalMoodChange += rest_mood_const;
        }

        currentMoodEffect.Value = totalMoodChange;
        _netMoodChange.Value = totalMoodChange;
    }
}