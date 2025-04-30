using UnityEngine;

[CreateAssetMenu(fileName = "DataLibrary", menuName = "Game/DataLibrary")]
public class DataLibrary : ScriptableObject
{
    [Header("Апгрейды")]
    public UpgradeDataSO upgradeData;
    [Header("Донаты")]
    public DonateUpgradesSO donateData;
    [Header("Ресурсы")]
    public CargoResourceProductionDataSO resourceDropData;
    [Header("Библиотека звуков")]
    public SoundLibrarySO soundLibrary;
    [Header("Ежедневные награды")]
    public DailyRewardsConfig dailyRewardsConfig;
}