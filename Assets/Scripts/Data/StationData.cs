using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Unity.Services.CloudSave.Models;

[Serializable]
public class StationData
{
    public ReactiveProperty<int> MaxCrew { get; private set; }
    public Dictionary<Department, StationBlockData> DepartmentData = new();

    public StationData()
    {
        MaxCrew = new ReactiveProperty<int>(5); // Дефолтное значение
    }

    public bool IsUnlocked(Department dept)
    {
        return DepartmentData.ContainsKey(dept);
    }

    public void Unlock(Department dept)
    {
        if (!IsUnlocked(dept))
            DepartmentData[dept] = new StationBlockData();
    }

    public void Lock(Department dept)
    {
        DepartmentData.Remove(dept);
    }

    public int GetWorkbenchesLevelUnlocked(Department dept)
    {
        return DepartmentData.TryGetValue(dept, out var data) ? data.WorkBenchesInstalled : 0;
    }

    public void SetWorkbenchesLevelUnlocked(Department dept, int level)
    {
        if (DepartmentData.TryGetValue(dept, out var data))
            data.WorkBenchesInstalled = level;
    }

    public int GetMaxCrewUnlocked(Department dept)
    {
        return DepartmentData.TryGetValue(dept, out var data) ? data.MaxCrewUnlocked : 0;
    }

    public void SetMaxCrewUnlocked(Department dept, int maxCrew)
    {
        if (DepartmentData.TryGetValue(dept, out var data))
            data.MaxCrewUnlocked = maxCrew;
    }

    public int GetCurrentCrewHired(Department dept)
    {
        return DepartmentData.TryGetValue(dept, out var data) ? data.CurrentCrewHired : 0;
    }

    public void SetCurrentCrewHired(Department dept, int crewCount)
    {
        if (DepartmentData.TryGetValue(dept, out var data))
            data.CurrentCrewHired = crewCount;
    }

    // Метод для сериализации данных одного департамента в словарь
    public Dictionary<string, object> SerializeDepartment(Department dept)
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        if (DepartmentData.TryGetValue(dept, out var data))
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
                    stationData.DepartmentData[dept] = JsonUtility.FromJson<StationBlockData>(departmentItem.Value.GetAsString());
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
        dict["maxCrew"] = MaxCrew.Value; // int

        foreach (var pair in DepartmentData)
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
            stationData.MaxCrew.Value = maxCrewItem.Value.GetAs<int>();
        }
        else
        {
            stationData.MaxCrew.Value = 5;
        }

        foreach (var pair in dict)
        {
            if (pair.Key.StartsWith("department_") && Enum.TryParse<Department>(pair.Key.Substring("department_".Length), out var department))
            {
                if (pair.Value is Item departmentItem)
                {
                    try
                    {
                        stationData.DepartmentData[department] = JsonUtility.FromJson<StationBlockData>(departmentItem.Value.GetAsString());
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