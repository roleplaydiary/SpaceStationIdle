using UnityEngine;

public class DepartmentUpgradesScrollController : MonoBehaviour
{
    [SerializeField] private GameObject bridgeUpgrades;
    [SerializeField] private GameObject scienceUpgrades;
    [SerializeField] private GameObject cargoUpgrades;

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
            case Department.Science:
            {
                scienceUpgrades.SetActive(true);
                break;
            }
            case Department.Cargo:
            {
                cargoUpgrades.SetActive(true);
                break;
            }
        }
    }

    private void HideAllUpgrades()
    {
        bridgeUpgrades.SetActive(false);
        scienceUpgrades.SetActive(false);
        cargoUpgrades.SetActive(false);
    }
}
