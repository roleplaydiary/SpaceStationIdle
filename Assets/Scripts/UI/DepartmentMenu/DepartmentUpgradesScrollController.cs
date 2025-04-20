using UnityEngine;

public class DepartmentUpgradesScrollController : MonoBehaviour
{
    [SerializeField] private GameObject bridgeUpgrades;

    public void Initialize(Department department)
    {
        HideAllUpgrades();
        
        switch (department)
        {
            case Department.Bridge:
            {
                bridgeUpgrades.SetActive(true);
                break;
            }
        }
    }

    private void HideAllUpgrades()
    {
        bridgeUpgrades.SetActive(false);
    }
}
