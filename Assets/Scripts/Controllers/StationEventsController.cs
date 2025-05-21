using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class StationEventsController : MonoBehaviour
{
    private const float MIN_EVENT_DELAY = 60f; 
    private const float MAX_EVENT_DELAY = 180f; 
    
    private float medicalEventRatio = 10f;
    private float securityEventRatio = 10f;
    //при инициализации смотрим, сколько сотрудников, высчитываем, какая вероятность событий этих отделов
    // 1 сотрудник отдела должен иметь эквивалент 5 сотрудникам на станции. Тогда вероятность минимальна - 5 или 10%.
    // Максимальная вероятность опасности - 50%.
    // Может быть, добавить инженеров и учёных(перебои с энергией и сбежавшая аномалия)
    
    public readonly Subject<StationEventType> OnBonusEventStarted = new Subject<StationEventType>();
    public readonly Subject<MajorEventData> OnMajorEventStarted = new Subject<MajorEventData>();
    
    private IDisposable eventTimerSubscription;
    
    private void Awake()
    {
        ServiceLocator.Register(this);
    }
    
    private void OnDestroy()
    {
        eventTimerSubscription?.Dispose();
    }

    public void Initialize()
    {
        StartStationEventTimer();
    }

    private void StartStationEventTimer()
    {
        float randomDelay = Random.Range(MIN_EVENT_DELAY, MAX_EVENT_DELAY);
        Debug.Log($"Следующее событие (UniRx) произойдет через {randomDelay} секунд.");

        // Отписываемся от предыдущей подписки, если она есть
        eventTimerSubscription?.Dispose();

        // Создаем Observable, который сработает через случайное время
        eventTimerSubscription = Observable.Timer(TimeSpan.FromSeconds(randomDelay))
            .Subscribe(_ =>
            {
                // Вызывается по истечении времени
                RandomizeAndInitializeEvent();

                // После обработки события запускаем новый таймер
                StartStationEventTimer();
            });
    }
    
    private void RandomizeAndInitializeEvent()
    {
        StationEventType eventType = RandomizeStationEvent();
        Debug.Log($"Произошло событие (UniRx): {eventType}");

        switch (eventType)
        {
            case StationEventType.BonusEvent:
                BonusEventInitialize();
                break;
            case StationEventType.MinorEvent:
                MinorEventInitialize();
                break;
            case StationEventType.MajorEvent:
                MajorEventInitialize();
                break;
            case StationEventType.None:
                Debug.Log("Ничего не произошло (UniRx).");
                break;
        }
    }

    private StationEventType RandomizeStationEvent()
    {
        float randomValue = Random.Range(0f, 100f);
        
        if (randomValue < 50f)
        {
            return StationEventType.BonusEvent;
        }
        else if (randomValue < 60f)
        {
            return StationEventType.MinorEvent;
        }
        else if (randomValue < 65f)
        {
            return StationEventType.MajorEvent;
        }
        
        return StationEventType.None;
    }
    

    private void BonusEventInitialize()
    {
        OnBonusEventStarted.OnNext(StationEventType.BonusEvent);
        Debug.Log("Randomized Station Event: Bonus Event");
    }

    private void MinorEventInitialize()
    {
        //OnBonusEventStarted.OnNext(StationEventType.MinorEvent);
        Debug.Log("Randomized Station Event: Minor Event");
    }

    private void MajorEventInitialize()
    {
        MajorEventData newMajorEvent = new MajorEventData()
        {
            Department = Department.Engineering, 
            StationMajorEventType = StationMajorEventType.FireHazard
        };
        //TODO: Randomize department and event type
        
        OnMajorEventStarted.OnNext(newMajorEvent);
        Debug.Log("Randomized Station Event: Major Event");
    }

    public void TestEvent()
    {
        RandomizeAndInitializeEvent();
    }

    public void EngineeringFireHazardTest()
    {
        MajorEventData newMajorEvent = new MajorEventData()
        {
            Department = Department.Engineering, 
            StationMajorEventType = StationMajorEventType.FireHazard
        };
        OnMajorEventStarted.OnNext(newMajorEvent);
    }

    public void MajorEventFinish(Department department)
    {
        MajorEventData newMajorEvent = new MajorEventData()
        {
            Department = department, 
            StationMajorEventType = StationMajorEventType.None
        };
        OnMajorEventStarted.OnNext(newMajorEvent);
    }
}

public enum StationEventType
{
    None,
    BonusEvent,//50%
    MinorEvent,//20%
    MajorEvent,//5%
}

public enum StationMajorEventType
{
    None,
    AsteroidHit,
    FireHazard,
    Aliens,
    Epidemic,
    Anomaly
}

public struct MajorEventData
{
    public Department Department;  
    public StationMajorEventType StationMajorEventType;
}
