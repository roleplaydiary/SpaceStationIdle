using System;
using System.Collections.Generic;
using UniRx;

[Serializable]
public class PlayerData
{
    public ReactiveProperty<float> playerCredits { get; private set; }
    public ReactiveProperty<float> researchPoints { get; private set; }

    public PlayerData()
    {
        playerCredits = new ReactiveProperty<float>();
        researchPoints = new ReactiveProperty<float>();
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "playerCredits", playerCredits.Value },
            { "researchPoints", researchPoints.Value }
        };
    }

    public static PlayerData FromDictionary(Dictionary<string, object> dict)
    {
        PlayerData playerData = new PlayerData();
        if (dict.TryGetValue("playerCredits", out object credits))
            playerData.playerCredits.Value = Convert.ToSingle(credits);
        if (dict.TryGetValue("researchPoints", out object points))
            playerData.researchPoints.Value = Convert.ToSingle(points);
        return playerData;
    }
}