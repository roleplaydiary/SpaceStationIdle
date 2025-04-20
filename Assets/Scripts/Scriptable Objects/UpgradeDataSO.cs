using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Game Data/Upgrade Data")]
public class UpgradeDataSO : ScriptableObject
{
    [System.Serializable]
    public struct UpgradeEntry
    {
        public string upgradeId; // Уникальный идентификатор апгрейда (например, "bridge_max_crew_level_1")
        public string displayName; // Отображаемое имя апгрейда
        [TextArea] public string description; // Описание апгрейда
        public UpgradeType type; // Тип апгрейда (см. enum ниже)
        public int level; // Уровень апгрейда (если есть несколько уровней)
        public UpgradeCost cost;
    }

    [System.Serializable]
    public struct UpgradeCost
    {
        public float credits;
        public int researchPoints;
        public Resources resources;
    }

    public enum UpgradeType
    {
        DepartmentMaxCrew,
        StationMaxCrew,
        DepartmentMaxWorkbenches,
        // Другие типы апгрейдов
    }

    public UpgradeEntry[] upgrades;

    // Метод для поиска апгрейда по ID (может пригодиться)
    public UpgradeEntry GetUpgradeById(string id)
    {
        foreach (var upgrade in upgrades)
        {
            if (upgrade.upgradeId == id)
            {
                return upgrade;
            }
        }
        return default; // Или можно вернуть null и обрабатывать это
    }
}