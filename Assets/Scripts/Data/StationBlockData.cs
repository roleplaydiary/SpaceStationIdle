using UnityEngine.Serialization;

[System.Serializable]
public class StationBlockData 
{
    public int WorkStationsInstalled = 1;
    public int WorkStationsMax = 1;
    public int MaxCrewUnlocked = 1;
    public int CurrentCrewHired = 0;
    public int CrewAtWork = 0;
    public int CrewAtRest = 0;
    //public List<CharacterData> character = new List<CharacterData>(); // Список сотрудников в отделе
    //Сюда же добавить ресёрчи
}