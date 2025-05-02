using System.Collections.Generic;
using UniRx;
using System;
using System.Linq;
using UnityEngine;

public class StationMoodService : IDisposable
{
    private ReactiveProperty<float> currentStationMood = new ReactiveProperty<float>(100f); // Дефолтное настроение - 100
    public IReadOnlyReactiveProperty<float> CurrentStationMood => currentStationMood;

    private List<IDepartmentMoodUser> moodUsers = new List<IDepartmentMoodUser>();
    private CompositeDisposable disposables = new CompositeDisposable();
    private const float defaultStationMood = 100f;

    public static float negativeProductionRate = 0.1f;
    public static float negativeProductionValue = 10;
    public static float positiveProductionRate = 1.1f;
    public static float positiveProductionValue = 100;
    public StationMoodService()
    {
        // Подписываемся на изменения настроения от всех пользователей
        CreateMoodSubscription();
    }

    private void CreateMoodSubscription()
    {
        // Отписываемся от старой подписки
        disposables.Clear();
        disposables = new CompositeDisposable();

        // Получаем массив IObservable<float> для каждого пользователя
        var moodChangeStreams = moodUsers.Select(user => user.NetMoodChange.AsObservable()).ToArray();

        if (moodChangeStreams.Length > 0)
        {
            Observable.CombineLatest(moodChangeStreams)
                .Subscribe(changes =>
                {
                    // Суммируем все изменения и прибавляем к базовому настроению
                    currentStationMood.Value = defaultStationMood + changes.Sum();
                    // Больше нет Mathf.Clamp
                    if (currentStationMood.Value < 20f) // Пример порога низкого настроения (можно скорректировать)
                    {
                        Debug.LogWarning("Низкое настроение на станции!");
                    }
                })
                .AddTo(disposables);
        }
        else
        {
            // Если нет пользователей настроения, возвращаем дефолтное значение
            currentStationMood.Value = defaultStationMood;
        }
    }

    public DepartmentMoodInfo GetDepartmentMoodInfo(Department department)
    {
        float totalMoodChange = 0f;
        // int crewAtWork = 0;

        // StationController stationController = ServiceLocator.Get<StationController>();
        // if (stationController != null && stationController.StationData != null && stationController.StationData.departmentData.TryGetValue(department, out var value))
        // {
        //     crewAtWork = value.CrewAtWork;
        // }

        foreach (var user in moodUsers)
        {
            if (user is DepartmentMoodController departmentController &&
                departmentController.GetComponent<StationBlockController>()?.GetBlockType() == department) // TODO: избавиться от getComponent в будущем
            {
                totalMoodChange += departmentController.currentMoodEffect.Value;
                break; // Предполагаем, что на один департамент приходится один DepartmentMoodController
            }
        }

        // Базовое настроение станции + суммарный эффект для данного департамента
        float departmentMoodLevel = totalMoodChange;

        return new DepartmentMoodInfo(departmentMoodLevel);
    }

    public void RegisterMoodUser(IDepartmentMoodUser user)
    {
        if (!moodUsers.Contains(user))
        {
            moodUsers.Add(user);
            // При регистрации нужно пересоздать подписку CombineLatest
            CreateMoodSubscription();
            // Debug.Log($"Зарегистрирован пользователь настроения: {user.GetType().Name}");
        }
    }

    public void UnregisterMoodUser(IDepartmentMoodUser user)
    {
        if (moodUsers.Contains(user))
        {
            moodUsers.Remove(user);
            CreateMoodSubscription();
            // Debug.Log($"Отменен пользователь настроения: {user.GetType().Name}");
        }
    }

    public void Dispose()
    {
        disposables.Clear();
    }
}

public interface IDepartmentMoodUser
{
    IReadOnlyReactiveProperty<float> NetMoodChange { get; }
}

public struct DepartmentMoodInfo
{
    public float MoodEffect { get; }

    public DepartmentMoodInfo(float moodEffect)
    {
        MoodEffect = moodEffect;
    }
}