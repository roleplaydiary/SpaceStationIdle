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
    
    public readonly BehaviorSubject<StationEventType> OnEventStarted = new BehaviorSubject<StationEventType>(StationEventType.None);
    
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
        OnEventStarted.OnNext(StationEventType.BonusEvent);
        Debug.Log("Randomized Station Event: Bonus Event");
    }

    private void MinorEventInitialize()
    {
        OnEventStarted.OnNext(StationEventType.MinorEvent);
        Debug.Log("Randomized Station Event: Minor Event");
    }

    private void MajorEventInitialize()
    {
        OnEventStarted.OnNext(StationEventType.MajorEvent);
        Debug.Log("Randomized Station Event: Major Event");
    }

    public void TestEvent()
    {
        RandomizeAndInitializeEvent();
    }
}

public enum StationEventType
{
    None,
    BonusEvent,//50%
    MinorEvent,//20%
    MajorEvent,//5%
}
