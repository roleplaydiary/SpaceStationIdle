using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    private void Awake()
    {
        ServiceLocator.Register(this);
    }
    
    public async Task Autentication()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Аутентификация прошла успешно.");
        }
        catch(Exception e)
        {
            Debug.LogError("Authentication failed: " + e);
        }
    }
    
    public async Task<PlayerData> LoadPlayerData()
    {
        try
        {
            // Указываем ключи, которые хотим загрузить
            HashSet<string> keysToLoad = new HashSet<string> {
                "playerCredits", "maxCrew", "crewMood"
            };

            // Загружаем данные из Cloud Save
            var playerDataDict = await CloudSaveService.Instance.Data.Player.LoadAsync(keysToLoad);

            // Преобразуем данные из Dictionary в PlayerData
            PlayerData playerData = new PlayerData();

            if (playerDataDict != null)
            {
                if (playerDataDict.TryGetValue("playerCredits", out var value2))
                    playerData.playerCredits = Convert.ToInt32(value2);

                if (playerDataDict.TryGetValue("maxCrew", out var value1))
                    playerData.maxCrew = Convert.ToInt32(value1);

                if (playerDataDict.TryGetValue("crewMood", out var value))
                    playerData.crewMood = Convert.ToSingle(value);
            }

            Debug.Log("Данные игрока успешно загружены из Cloud Save.");
            return playerData;
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка загрузки данных игрока: {e.Message}");
            return null;
        }
    }

    public async Task SavePlayerData(PlayerData playerData)
    {
        try
        {
            Dictionary<string, object> playerDataDict = new Dictionary<string, object>
            {
                { "playerCredits", playerData.playerCredits },
                { "maxCrew", playerData.maxCrew },
                { "crewMood", playerData.crewMood }
            };

            // Сохраняем данные в Cloud Save
            await CloudSaveService.Instance.Data.Player.SaveAsync(playerDataDict);

            Debug.Log("Данные игрока успешно сохранены в Cloud Save.");
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка сохранения данных игрока: {e.Message}");
        }
    }

    public void LoadStationData()
    {
        
    }

    public void SaveStationData()
    {
        
    }
}
