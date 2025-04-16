using System;
using System.Collections.Generic;
using UniRx;

[Serializable]
public class PlayerData
{
    public ReactiveProperty<int> playerCredits { get; private set; }

    // Конструктор для инициализации ReactiveProperty
    public PlayerData()
    {
        playerCredits = new ReactiveProperty<int>();
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "playerCredits", playerCredits.Value }
        };
    }

    public static PlayerData FromDictionary(Dictionary<string, object> dict)
    {
        PlayerData playerData = new PlayerData();
        if (dict.TryGetValue("playerCredits", out object credits))
            playerData.playerCredits.Value = Convert.ToInt32(credits);
        return playerData;
    }
}