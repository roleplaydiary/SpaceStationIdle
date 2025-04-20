using System;
using System.Collections.Generic;
using UniRx;

[Serializable]
public class PlayerData
{
    public ReactiveProperty<float> playerCredits { get; private set; }
    public ReactiveProperty<int> researchPoints { get; private set; }

    public PlayerData()
    {
        playerCredits = new ReactiveProperty<float>();
        researchPoints = new ReactiveProperty<int>();
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
            playerData.researchPoints.Value = Convert.ToInt32(points);
        return playerData;
    }
}