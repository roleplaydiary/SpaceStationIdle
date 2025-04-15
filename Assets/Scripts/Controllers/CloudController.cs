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
    
    public async Task SavePlayerData(PlayerData playerData)
    {
        try
        {
            Dictionary<string, object> playerDataDict = playerData.ToDictionary();
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> { { "player_data", playerDataDict } });
            Debug.Log("Данные игрока успешно сохранены в Cloud Save.");
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка сохранения данных игрока: {e.Message}");
        }
    }

    public async Task<PlayerData> LoadPlayerData()
    {
        try
        {
            var playerDataItems = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "player_data" });
            if (playerDataItems != null && playerDataItems.TryGetValue("player_data", out var playerDataItem))
            {
                Dictionary<string, object> playerDataDict = (Dictionary<string, object>)playerDataItem.Value.GetAs<Dictionary<string, object>>();
                return PlayerData.FromDictionary(playerDataDict);
            }
            return null;
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка загрузки данных игрока: {e.Message}");
            return null;
        }
    }

    public async Task<StationData> LoadStationData()
    {
        try
        {
            HashSet<string> keysToLoad = new HashSet<string>();
            foreach (Department department in Enum.GetValues(typeof(Department)))
            {
                keysToLoad.Add(department.ToString());
            }

            var stationDataItems = await CloudSaveService.Instance.Data.Player.LoadAsync(keysToLoad);
            if (stationDataItems != null)
            {
                Dictionary<string, object> stationDataDict = new Dictionary<string, object>();
                foreach (var pair in stationDataItems)
                {
                    stationDataDict[pair.Key] = pair.Value.Value.GetAsString();
                }
                return StationData.FromDictionary(stationDataDict);
            }
            return null;
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка загрузки данных станции: {e.Message}");
            return null;
        }
    }

    public async Task SaveStationData(StationData stationData)
    {
        try
        {
            Dictionary<string, object> stationDataDict = stationData.ToDictionary();
            await CloudSaveService.Instance.Data.Player.SaveAsync(stationDataDict);
            Debug.Log("Данные станции успешно сохранены в Cloud Save.");
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка сохранения данных станции: {e.Message}");
        }
    }
    
    public async Task SaveResources(Resources resources)
    {
        try
        {
            string resourcesJson = JsonUtility.ToJson(resources);
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> { { "resources", resourcesJson } });
            Debug.Log("Ресурсы успешно сохранены в Cloud Save.");
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка сохранения ресурсов: {e.Message}");
        }
    }

    public async Task<Resources> LoadResources()
    {
        try
        {
            var resourcesItems = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "resources" });
            if (resourcesItems != null && resourcesItems.TryGetValue("resources", out var resourcesItem))
            {
                return JsonUtility.FromJson<Resources>(resourcesItem.Value.GetAsString());
            }
            return null;
        }
        catch (CloudSaveException e)
        {
            Debug.LogError($"Ошибка загрузки ресурсов: {e.Message}");
            return null;
        }
    }
}
