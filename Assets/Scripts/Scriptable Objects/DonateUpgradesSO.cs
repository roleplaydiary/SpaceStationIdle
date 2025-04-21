using UnityEngine;

[CreateAssetMenu(fileName = "DonateUpgradeData", menuName = "Game Data/Donate Upgrade Data")]
public class DonateUpgradesSO : ScriptableObject
{
    [System.Serializable]
    public struct UpgradeEntry
    {
        public string upgradeId; // Уникальный идентификатор апгрейда
        public string displayName; // Отображаемое имя апгрейда
        [TextArea] public string description; // Описание апгрейда
        public UpgradeType type; // Тип апгрейда (см. enum ниже)
        public int value;
        public int level; // Уровень апгрейда (если есть несколько уровней)
        public int cost;
    }

    public enum UpgradeType
    {
        AFKTime
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
