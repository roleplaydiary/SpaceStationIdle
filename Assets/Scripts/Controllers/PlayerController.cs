// PlayerController.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;
using System;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    public PlayerData PlayerData { get => playerData; }

    private void Awake()
    {
        ServiceLocator.Register(this);
    }
    
    private void Start()
    {
        AutoSaveTimer();
    }

    private void AutoSaveTimer()
    {
        Observable
            .Interval(TimeSpan.FromMinutes(2))
            .Subscribe(async _ =>
            {
                await SavePlayerData();
            })
            .AddTo(this);
    }


    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public async Task PlayerInitialization()
    {
        var loadData = await LoadPlayerData();
        if (loadData == null)
        {
            loadData = new PlayerData();
            loadData.playerCredits.Value = 30;
            loadData.researchPoints.Value = 0;
        }
        playerData = loadData;
    }

    public async Task SavePlayerData()
    {
        playerData.lastSaveTime = DateTime.UtcNow; // Обновляем время сохранения перед записью
        await ServiceLocator.Get<CloudController>().SavePlayerData(playerData);
    }

    private async Task<PlayerData> LoadPlayerData()
    {
        var loadData = await ServiceLocator.Get<CloudController>().LoadPlayerData();
        return loadData;
    }

    // Пример изменения значения Player Credits извне
    public void AddCredits(float amount) // Изменил на float, чтобы соответствовать ReactiveProperty<float>
    {
        playerData.playerCredits.Value += amount;
    }

    public void AddResearchPoints(float amount) // Изменил на float
    {
        playerData.researchPoints.Value += amount;
    }

    public DateTime GetLastSaveTime()
    {
        return playerData.lastSaveTime;
    }
}