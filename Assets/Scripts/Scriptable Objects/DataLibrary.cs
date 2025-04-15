using UnityEngine;

[CreateAssetMenu(fileName = "DataLibrary", menuName = "Game/DataLibrary")]
public class DataLibrary : ScriptableObject
{
    [Header("Персонажи")]
    public GameObject[] characterPrefabs;

    // Можно добавить другие данные: станции, предметы и т.п.
}