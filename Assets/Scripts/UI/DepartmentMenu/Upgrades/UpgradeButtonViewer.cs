using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeButtonViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text price;
    private UpgradeDataSO upgradeData;

    public void Initialize(string upgradeId)
    {
        if (upgradeId == null)
            return;
        
        upgradeData = ServiceLocator.Get<DataLibrary>().upgradeData;
        var upgrade = upgradeData.GetUpgradeById(upgradeId);
        title.text = upgrade.displayName;
        price.text = "";
        
        if (upgrade.upgradeId == null)
        {
            Debug.Log("Upgrade doesn't exist " + upgradeId);
            return;
        }
        
        if (upgrade.cost.credits > 0)
        {
            price.text += $"Credits: {upgrade.cost.credits}";
        }

        if (upgrade.cost.researchPoints > 0)
        {
            price.text += $"\nResearch Points: {upgrade.cost.researchPoints}";
        }
        
        foreach (var resoruce in upgrade.cost.resources)
        {
            if (resoruce.Value > 0)
            {
                price.text += $"\n{resoruce.Key}: {resoruce.Value}";
            }
        }
    }
}
