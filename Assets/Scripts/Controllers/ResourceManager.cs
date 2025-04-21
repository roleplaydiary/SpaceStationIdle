using System;
using System.Threading.Tasks;
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

    public float GetResourceAmount(string key)
    {
        switch (key)
        {
            case "Phoron":
                return currentResources.Value.Phoron;
            case "Metal":
                return currentResources.Value.Metal;
            case "Glass":
                return currentResources.Value.Glass;
            case "Plastic":
                return currentResources.Value.Plastic;
            case "Gold":
                return currentResources.Value.Gold;
            case "Silver":
                return currentResources.Value.Silver;
            case "Uranium":
                return currentResources.Value.Uranium;
            default:
                Debug.LogError($"Неизвестный тип ресурса: {key}");
                return 0f;
        }
    }

    public Resources GetCurrentResources()
    {
        return currentResources.Value;
    }

    public void AddResource(string type, float amount)
    {
        Resources newResources = currentResources.Value;
        switch (type)
        {
            case "Phoron":
                newResources.Phoron += amount;
                break;
            case "Metal":
                newResources.Metal += amount;
                break;
            case "Glass":
                newResources.Glass += amount;
                break;
            case "Plastic":
                newResources.Plastic += amount;
                break;
            case "Gold":
                newResources.Gold += amount;
                break;
            case "Silver":
                newResources.Silver += amount;
                break;
            case "Uranium":
                newResources.Uranium += amount;
                break;
            default:
                Debug.LogError($"Неизвестный тип ресурса: {type}");
                return;
        }
        currentResources.Value = newResources;
        //Сохранение ресурсов было вынесено отсюда, чтобы в цикле не сохранять по сто раз
    }

    public bool TryRemoveResource(string type, float amount)
    {
        Resources current = currentResources.Value;
        float currentAmount;

        switch (type)
        {
            case "Phoron":
                currentAmount = current.Phoron;
                break;
            case "Metal":
                currentAmount = current.Metal;
                break;
            case "Glass":
                currentAmount = current.Glass;
                break;
            case "Plastic":
                currentAmount = current.Plastic;
                break;
            case "Gold":
                currentAmount = current.Gold;
                break;
            case "Silver":
                currentAmount = current.Silver;
                break;
            case "Uranium":
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

    public async Task SaveResources()
    {
        await ServiceLocator.Get<CloudController>().SaveResources(currentResources.Value);
        Debug.Log($"Ресурсы сохранены: {currentResources.Value.Phoron}, {currentResources.Value.Metal}, ...");
    }

    public async void LoadResources()
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
