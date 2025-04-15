using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int playerCredits;
    public int maxCrew;
    public float crewMood;
    
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "playerCredits", playerCredits },
            { "maxCrew", maxCrew },
            { "crewMood", crewMood }
        };
    }

    public static PlayerData FromDictionary(Dictionary<string, object> dict)
    {
        PlayerData playerData = new PlayerData();
        if (dict.TryGetValue("playerCredits", out object credits))
            playerData.playerCredits = Convert.ToInt32(credits);
        if (dict.TryGetValue("maxCrew", out object maxCrew))
            playerData.maxCrew = Convert.ToInt32(maxCrew);
        if (dict.TryGetValue("crewMood", out object crewMood))
            playerData.crewMood = Convert.ToSingle(crewMood);
        return playerData;
    }
}
