using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.Services.CloudSave.Models;

[Serializable]
public class StationData
{
    public ReactiveProperty<int> maxCrew { get; private set; }
    public ReactiveProperty<float> crewMood { get; private set; }
    public Dictionary<Department, StationBlockData> departmentData = new();

    public StationData()
    {
        maxCrew = new ReactiveProperty<int>(5); // Дефолтное значение
        crewMood = new ReactiveProperty<float>(1000f); // Дефолтное значение
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

    // Метод для сериализации данных одного департамента в словарь
    public Dictionary<string, object> SerializeDepartment(Department dept)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        if (departmentData.TryGetValue(dept, out var data))
        {
            string json = JsonUtility.ToJson(data);
            dict[$"department_{dept}"] = json;
            Debug.Log($"Сериализован department_{dept}: {json}");
        }
        return dict;
    }

    // Статический метод для десериализации данных одного департамента из Item
    public static bool TryDeserializeDepartment(Dictionary<string, Item> dict, Department dept, StationData stationData)
    {
        if (dict.TryGetValue($"department_{dept}", out Item departmentItem))
        {
            if (departmentItem != null)
            {
                try
                {
                    stationData.departmentData[dept] = JsonUtility.FromJson<StationBlockData>(departmentItem.Value.GetAsString());
                    Debug.Log($"Успешно десериализован отдел: {dept}, JSON: {departmentItem.Value.GetAsString()}");
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Ошибка при десериализации отдела {dept}: {e.Message}, JSON: {departmentItem.Value.GetAsString()}");
                    return false;
                }
            }
            else
            {
                Debug.LogWarning($"Значение для ключа 'department_{dept}' является null Item.");
                return false;
            }
        }
        return false;
    }

    public Dictionary<string, object> ToDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict["maxCrew"] = maxCrew.Value; // int
        dict["crewMood"] = crewMood.Value; // float

        foreach (var pair in departmentData)
        {
            string json = JsonUtility.ToJson(pair.Value);
            dict[$"department_{pair.Key}"] = json;
            Debug.Log($"Сериализован department_{pair.Key}: {json}");
        }

        return dict;
    }

    public static StationData FromDictionary(Dictionary<string, Item> dict) // Принимаем Dictionary<string, Item>
    {
        StationData stationData = new StationData();

        if (dict.TryGetValue("maxCrew", out Item maxCrewItem))
        {
            stationData.maxCrew.Value = maxCrewItem.Value.GetAs<int>();
        }
        else
        {
            stationData.maxCrew.Value = 5;
        }

        if (dict.TryGetValue("crewMood", out Item crewMoodItem))
        {
            stationData.crewMood.Value = crewMoodItem.Value.GetAs<float>();
        }
        else
        {
            stationData.crewMood.Value = 1000f;
        }

        foreach (var pair in dict)
        {
            if (pair.Key.StartsWith("department_") && Enum.TryParse<Department>(pair.Key.Substring("department_".Length), out var department))
            {
                if (pair.Value is Item departmentItem)
                {
                    try
                    {
                        stationData.departmentData[department] = JsonUtility.FromJson<StationBlockData>(departmentItem.Value.GetAsString());
                        Debug.Log($"Успешно десериализован отдел: {department}, JSON: {departmentItem.Value.GetAsString()}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Ошибка при десериализации отдела {department}: {e.Message}, JSON: {departmentItem.Value.GetAsString()}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Значение для ключа '{pair.Key}' не является Item.");
                }
            }
        }

        return stationData;
    }
}