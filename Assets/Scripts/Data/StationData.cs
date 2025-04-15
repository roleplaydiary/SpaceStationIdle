using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StationData
{
    public List<Department> unlockedDepartments = new();

    public bool IsUnlocked(Department dept)
    {
        return unlockedDepartments.Contains(dept);
    }

    public void Unlock(Department dept)
    {
        if (!unlockedDepartments.Contains(dept))
            unlockedDepartments.Add(dept);
    }

    public void Lock(Department dept)
    {
        if (unlockedDepartments.Contains(dept))
            unlockedDepartments.Remove(dept);
    }
}