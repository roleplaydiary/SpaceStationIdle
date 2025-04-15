using UnityEngine;

[CreateAssetMenu(fileName = "New Station Block", menuName = "Space Station/Station Block")]
public class StationBlockDataSO : ScriptableObject
{
    public string BlockName;
    public int MaxCrew;
    public Department BlockType;
}