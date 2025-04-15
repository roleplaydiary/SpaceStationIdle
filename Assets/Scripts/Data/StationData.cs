using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationData
{
    public Dictionary<Department, StationBlockData> departmentData = new();

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
        foreach (var pair in departmentData)
        {
            dict[pair.Key.ToString()] = JsonUtility.ToJson(pair.Value);
        }
        return dict;
    }

    public static StationData FromDictionary(Dictionary<string, object> dict)
    {
        StationData stationData = new StationData();
        foreach (var pair in dict)
        {
            if (Enum.TryParse(pair.Key, out Department department))
            {
                stationData.departmentData[department] = JsonUtility.FromJson<StationBlockData>(pair.Value.ToString());
            }
        }
        return stationData;
    }
}