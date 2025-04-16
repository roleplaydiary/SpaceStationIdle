using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[Serializable]
public class StationData
{
    public ReactiveProperty<int> maxCrew { get; private set; }
    public ReactiveProperty<float> crewMood { get; private set; }
    public Dictionary<Department, StationBlockData> departmentData = new();

    public StationData()
    {
        maxCrew = new ReactiveProperty<int>(5);
        crewMood = new ReactiveProperty<float>(1000f);
    }

    public bool IsUnlocked(Department dept)
    {
        return departmentData.ContainsKey(dept);
    }

    public void Unlock(Department dept)
    {
        if (!IsUnlocked(dept))
            departmentData[dept] = new StationBlockData();
    }

    public void Lock(Department dept)
    {
        departmentData.Remove(dept);
    }

    public int GetWorkbenchesLevelUnlocked(Department dept)
    {
        return departmentData.TryGetValue(dept, out var data) ? data.WorkBenchesLevelUnlocked : 0;
    }

    public void SetWorkbenchesLevelUnlocked(Department dept, int level)
    {
        if (departmentData.TryGetValue(dept, out var data))
            data.WorkBenchesLevelUnlocked = level;
    }

    public int GetMaxCrewUnlocked(Department dept)
    {
        return departmentData.TryGetValue(dept, out var data) ? data.MaxCrewUnlocked : 0;
    }

    public void SetMaxCrewUnlocked(Department dept, int maxCrew)
    {
        if (departmentData.TryGetValue(dept, out var data))
            data.MaxCrewUnlocked = maxCrew;
    }

    public int GetCurrentCrewHired(Department dept)
    {
        return departmentData.TryGetValue(dept, out var data) ? data.CurrentCrewHired : 0;
    }

    public void SetCurrentCrewHired(Department dept, int crewCount)
    {
        if (departmentData.TryGetValue(dept, out var data))
            data.CurrentCrewHired = crewCount;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict["maxCrew"] = maxCrew.Value;
        dict["crewMood"] = crewMood.Value;

        Dictionary<string, object> departmentDict = new Dictionary<string, object>();
        foreach (var pair in departmentData)
        {
            departmentDict[pair.Key.ToString()] = JsonUtility.ToJson(pair.Value);
        }
        dict["departmentData"] = departmentDict;

        return dict;
    }

    public static StationData FromDictionary(Dictionary<string, object> dict)
    {
        StationData stationData = new StationData();
        if (dict.TryGetValue("maxCrew", out object maxCrew))
            stationData.maxCrew.Value = Convert.ToInt32(maxCrew);
        if (dict.TryGetValue("crewMood", out object crewMood))
            stationData.crewMood.Value = Convert.ToSingle(crewMood);

        if (dict.TryGetValue("departmentData", out object departmentDataObj) && departmentDataObj is Dictionary<string, object> departmentDict)
        {
            foreach (var pair in departmentDict)
            {
                if (Enum.TryParse(pair.Key, out Department department))
                {
                    stationData.departmentData[department] = JsonUtility.FromJson<StationBlockData>(pair.Value.ToString());
                }
            }
        }
        return stationData;
    }
}