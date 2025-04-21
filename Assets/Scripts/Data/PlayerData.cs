using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public ReactiveProperty<float> playerCredits { get; private set; }
    public ReactiveProperty<float> researchPoints { get; private set; }
    public DateTime lastSaveTime;
    public ReactiveProperty<int> afkLevel = new ReactiveProperty<int>(0);

    public PlayerData()
    {
        playerCredits = new ReactiveProperty<float>();
        researchPoints = new ReactiveProperty<float>();
        lastSaveTime = DateTime.UtcNow;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "playerCredits", playerCredits.Value },
            { "researchPoints", researchPoints.Value },
            { "lastSaveTime", lastSaveTime.ToString() },
            { "afkLevel", afkLevel.Value }
        };
    }

    public static PlayerData FromDictionary(Dictionary<string, object> dict)
    {
        PlayerData playerData = new PlayerData();
        if (dict.TryGetValue("playerCredits", out object credits))
            playerData.playerCredits.Value = Convert.ToSingle(credits);
        if (dict.TryGetValue("researchPoints", out object points))
            playerData.researchPoints.Value = Convert.ToSingle(points);
        if (dict.TryGetValue("lastSaveTime", out object saveTime))
        {
            if (DateTime.TryParse(saveTime.ToString(), out DateTime parsedTime))
            {
                playerData.lastSaveTime = parsedTime;
            }
            else
            {
                playerData.lastSaveTime = DateTime.UtcNow;
                Debug.LogError($"Не удалось распарсить lastSaveTime: {saveTime}. Установлено текущее время.");
            }
        }
        else
        {
            playerData.lastSaveTime = DateTime.UtcNow;
        }
        if (dict.TryGetValue("afkLevel", out object afkLvl))
            playerData.afkLevel.Value = Convert.ToInt32(afkLvl);
        return playerData;
    }
}