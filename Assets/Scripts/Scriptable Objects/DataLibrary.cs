using UnityEngine;

[CreateAssetMenu(fileName = "DataLibrary", menuName = "Game/DataLibrary")]
public class DataLibrary : ScriptableObject
{
    [Header("Персонажи")]
    public GameObject[] characterPrefabs;
    [Header("Апгрейды")]
    public UpgradeDataSO upgradeData;
    [Header("Донаты")]
    public DonateUpgradesSO donateData;
    [Header("Ресурсы")]
    public CargoResourceProductionDataSO resourceDropData;
}