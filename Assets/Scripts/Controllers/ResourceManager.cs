using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ResourceManager : IDisposable
{
    private ReactiveProperty<Resources> currentResources = new ReactiveProperty<Resources>(new Resources());
    public IReadOnlyReactiveProperty<Resources> CurrentResources => currentResources;

    private CompositeDisposable disposables = new CompositeDisposable();

    public async Task InitializeAsync()
    {
        await LoadResources();
    }

    public float GetResourceAmount(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Phoron:
                return currentResources.Value.Phoron;
            case ResourceType.Metal:
                return currentResources.Value.Metal;
            case ResourceType.Glass:
                return currentResources.Value.Glass;
            case ResourceType.Plastic:
                return currentResources.Value.Plastic;
            case ResourceType.Gold:
                return currentResources.Value.Gold;
            case ResourceType.Silver:
                return currentResources.Value.Silver;
            case ResourceType.Uranium:
                return currentResources.Value.Uranium;
            default:
                Debug.LogError($"Неизвестный тип ресурса: {resourceType}");
                return 0f;
        }
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
        //Сохранение ресурсов было вынесено отсюда, чтобы в цикле не сохранять по сто раз
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

    public async Task SaveResources()
    {
        var resourcesToSave = currentResources.Value; // Создаем локальную копию
        await ServiceLocator.Get<CloudController>().SaveResources(resourcesToSave);
        Debug.Log($"Ресурсы сохранены: {resourcesToSave.Phoron}, {resourcesToSave.Metal}, {resourcesToSave.Glass}, {resourcesToSave.Plastic}, {resourcesToSave.Gold}, {resourcesToSave.Silver}, {resourcesToSave.Uranium}");
    }

    public async Task LoadResources()
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

    public static ResourceType GetResourceTypeByName(string name)
    {
        switch (name)
        {
            case "Phoron":
                return ResourceType.Phoron;
            case "Metal":
                return ResourceType.Metal;
            case "Glass":
                return ResourceType.Glass;
            case "Plastic":
                return ResourceType.Plastic;
            case "Gold":
                return ResourceType.Gold;
            case "Silver":
                return ResourceType.Silver;
            case "Uranium":
                return ResourceType.Uranium;
            default:
                Debug.LogError($"Неизвестный тип ресурса: {name}");
                return default;
        }
    }

    public void Dispose()
    {
        disposables.Clear();
    }
}
