using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    public PlayerData PlayerData { get => playerData; }
    [SerializeField] private List<Resources> resources;

    private void Awake()
    {
        ServiceLocator.Register(this);
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
            loadData.playerCredits.Value = 100;
        }
        playerData = loadData;

        // Пример подписки на изменения playerCredits
        playerData.playerCredits.Subscribe(credits =>
        {
            Debug.Log($"Player Credits изменились на: {credits}");
            // Здесь вы можете вызвать другие методы или обновить UI
        }).AddTo(this); // AddTo(this) для автоматической отписки при уничтожении объекта
    }

    private async Task<PlayerData> LoadPlayerData()
    {
        var loadData = await ServiceLocator.Get<CloudController>().LoadPlayerData();
        return loadData;
    }

    // Пример изменения значения Player Credits извне
    public void AddCredits(int amount)
    {
        playerData.playerCredits.Value += amount;
    }
}