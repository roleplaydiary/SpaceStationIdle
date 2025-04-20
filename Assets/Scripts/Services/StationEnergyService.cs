using System.Collections.Generic;
using UniRx;
using System;
using System.Linq;
using UnityEngine;

public class StationEnergyService : IDisposable
{
    private ReactiveProperty<float> currentStationEnergy = new ReactiveProperty<float>(0f);
    public IReadOnlyReactiveProperty<float> CurrentStationEnergy => currentStationEnergy;

    private List<IDepartmentEnergyUser> energyUsers = new List<IDepartmentEnergyUser>();
    private CompositeDisposable disposables = new CompositeDisposable();

    public StationEnergyService()
    {
        // Подписываемся на изменения от всех пользователей энергии
        CreateEnergySubscription();
    }

    private void CreateEnergySubscription()
    {
        // Отписываемся от старой подписки
        disposables.Clear();
        disposables = new CompositeDisposable();

        // Получаем массив IObservable<float> для каждого пользователя
        var energyChangeStreams = energyUsers.Select(user => user.NetEnergyChange.AsObservable()).ToArray();

        if (energyChangeStreams.Length > 0)
        {
            Observable.CombineLatest(energyChangeStreams)
                .Subscribe(changes =>
                {
                    currentStationEnergy.Value = changes.Sum();
                    // Логика обработки дефицита энергии
                    if (currentStationEnergy.Value <= 0)
                    {
                        Debug.LogWarning("Дефицит энергии на станции!");
                        // Предпринять какие-то действия
                    }
                })
                .AddTo(disposables);
        }
        else
        {
            // Если нет пользователей энергии, просто устанавливаем 0
            currentStationEnergy.Value = 0f;
        }
    }
    
    public DepartmentEnergyInfo GetDepartmentEnergyInfo(Department department)
    {
        float production = 0f;
        float consumption = 0f;

        foreach (var user in energyUsers)
        {
            if (user is DepartmentEnergyController departmentController &&
                departmentController.GetComponent<StationBlockController>()?.GetBlockType() == department) // TODO: избавиться от getComponent в  будущем
            {
                production += departmentController.currentEnergyProduction.Value;
                consumption += departmentController.currentEnergyConsumption.Value;
                break;
            }
        }

        return new DepartmentEnergyInfo(production, consumption);
    }

    public void RegisterEnergyUser(IDepartmentEnergyUser user)
    {
        if (!energyUsers.Contains(user))
        {
            energyUsers.Add(user);
            // При регистрации нужно пересоздать подписку CombineLatest
            CreateEnergySubscription();
            // Debug.Log($"Зарегистрирован пользователь энергии: {user.GetType().Name}");
        }
    }

    public void UnregisterEnergyUser(IDepartmentEnergyUser user)
    {
        if (energyUsers.Contains(user))
        {
            energyUsers.Remove(user);
            CreateEnergySubscription();
            // Debug.Log($"Отменен пользователь энергии: {user.GetType().Name}");
        }
    }

    public void Dispose()
    {
        disposables.Clear();
    }
}

public interface IDepartmentEnergyUser
{
    IReadOnlyReactiveProperty<float> NetEnergyChange { get; }
}

public struct DepartmentEnergyInfo
{
    public float Production { get; }
    public float Consumption { get; }

    public DepartmentEnergyInfo(float production, float consumption)
    {
        Production = production;
        Consumption = consumption;
    }
}