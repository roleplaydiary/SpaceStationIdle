using System;
using UniRx;
using UnityEngine;

public class ResourceManager : IDisposable
{
    private ReactiveProperty<Resources> currentResources = new ReactiveProperty<Resources>(new Resources());
    public IReadOnlyReactiveProperty<Resources> CurrentResources => currentResources;

    private CompositeDisposable disposables = new CompositeDisposable();

    public ResourceManager()
    {
        LoadResources();
    }

    public Resources GetCurrentResources()
    {
        return currentResources.Value;
    }

    public void AddResource(ResourceType type, float amount)
    {
        Resources newResources = currentResources.Value;
        switch (type)
        {
            case ResourceType.Phoron:
                newResources.Phoron += amount;
                break;
            case ResourceType.Metal:
                newResources.Metal += amount;
                break;
            case ResourceType.Glass:
                newResources.Glass += amount;
                break;
            case ResourceType.Plastic:
                newResources.Plastic += amount;
                break;
            case ResourceType.Gold:
                newResources.Gold += amount;
                break;
            case ResourceType.Silver:
                newResources.Silver += amount;
                break;
            case ResourceType.Uranium:
                newResources.Uranium += amount;
                break;
            default:
                Debug.LogError($"Неизвестный тип ресурса: {type}");
                return;
        }
        currentResources.Value = newResources;
        SaveResources(); // Сохраняем при изменении
    }

    public bool TryRemoveResource(ResourceType type, float amount)
    {
        Resources current = currentResources.Value;
        float currentAmount;

        switch (type)
        {
            case ResourceType.Phoron:
                currentAmount = current.Phoron;
                break;
            case ResourceType.Metal:
                currentAmount = current.Metal;
                break;
            case ResourceType.Glass:
                currentAmount = current.Glass;
                break;
            case ResourceType.Plastic:
                currentAmount = current.Plastic;
                break;
            case ResourceType.Gold:
                currentAmount = current.Gold;
                break;
            case ResourceType.Silver:
                currentAmount = current.Silver;
                break;
            case ResourceType.Uranium:
                currentAmount = current.Uranium;
                break;
            default:
                Debug.LogError($"Неизвестный тип ресурса: {type}");
                return false;
        }

        if (currentAmount >= amount)
        {
            AddResource(type, -amount); // Используем AddResource с отрицательным значением для вычитания
            return true;
        }
        return false;
    }

    private async void SaveResources()
    {
        await ServiceLocator.Get<CloudController>().SaveResources(currentResources.Value);
        Debug.Log($"Ресурсы сохранены: {currentResources.Value.Phoron}, {currentResources.Value.Metal}, ...");
    }

    private async void LoadResources()
    {
        Resources loadData = await ServiceLocator.Get<CloudController>().LoadResources();

        if (loadData != null)
        {
            currentResources.Value = loadData;
            Debug.Log($"Ресурсы загружены из облака: {currentResources.Value.Phoron}, {currentResources.Value.Metal}, {currentResources.Value.Glass}, {currentResources.Value.Plastic}, {currentResources.Value.Gold}, {currentResources.Value.Silver}, {currentResources.Value.Uranium}");
        }
        else
        {
            // Если загрузка не удалась, устанавливаем значения по умолчанию
            currentResources.Value = new Resources { Phoron = 0, Metal = 0, Glass = 0, Plastic = 0, Gold = 0, Silver = 0, Uranium = 0 };
            Debug.Log($"Не удалось загрузить ресурсы из облака, установлены значения по умолчанию: {currentResources.Value.Phoron}, {currentResources.Value.Metal}, {currentResources.Value.Glass}, {currentResources.Value.Plastic}, {currentResources.Value.Gold}, {currentResources.Value.Silver}, {currentResources.Value.Uranium}");
        }
    }

    public void Dispose()
    {
        disposables.Clear();
    }
}

// Enum для удобного обращения к типам ресурсов
public enum ResourceType
{
    Phoron,
    Metal,
    Glass,
    Plastic,
    Gold,
    Silver,
    Uranium
}