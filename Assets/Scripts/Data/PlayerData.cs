using System;
using System.Collections.Generic;
using UniRx;

[Serializable]
public class PlayerData
{
    public ReactiveProperty<int> playerCredits { get; private set; }
    public ReactiveProperty<int> maxCrew { get; private set; }
    public ReactiveProperty<float> crewMood { get; private set; }

    // Конструктор для инициализации ReactiveProperty
    public PlayerData()
    {
        playerCredits = new ReactiveProperty<int>();
        maxCrew = new ReactiveProperty<int>();
        crewMood = new ReactiveProperty<float>();
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "playerCredits", playerCredits.Value },
            { "maxCrew", maxCrew.Value },
            { "crewMood", crewMood.Value }
        };
    }

    public static PlayerData FromDictionary(Dictionary<string, object> dict)
    {
        PlayerData playerData = new PlayerData();
        if (dict.TryGetValue("playerCredits", out object credits))
            playerData.playerCredits.Value = Convert.ToInt32(credits);
        if (dict.TryGetValue("maxCrew", out object maxCrew))
            playerData.maxCrew.Value = Convert.ToInt32(maxCrew);
        if (dict.TryGetValue("crewMood", out object crewMood))
            playerData.crewMood.Value = Convert.ToSingle(crewMood);
        return playerData;
    }
}