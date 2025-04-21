using UnityEngine;
using UnityEngine.Serialization;

public class DepartmentProductionPanelController : MonoBehaviour
{
    [SerializeField] private DepartmentProductionViewer bridgeDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer engineeringDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer scienceDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer cargoDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer medbayDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer securityDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer barDepartmentProductionViewer;

    public void ProductivityPanelInit(Department department)
    {
        switch (department)
        {
            case Department.Bridge:
            {
                BridgeProductivityPanelInit();
                break;
            }

            case Department.Engineering:
            {
                EngineerProductivityPanelInit();
                break;
            }

            case Department.Science:
            {
                ScienceProductivityPanelInit();
                break;
            }

            case Department.Cargo:
            {
                CargoProductivityPanelInit();
                break;
            }

            case Department.Bar:
            {
                BarProductivityPanelInit();
                break;
            }

            default:
            {
                Debug.LogError($"Unknown department: {department}");
                break;
            }
        }
    }
    
    private void BridgeProductivityPanelInit()
    {
        CloseAllPanels();
        if (bridgeDepartmentProductionViewer == null)
        {
            Debug.LogError("Bridge production viewer is null");
            return;
        }
        bridgeDepartmentProductionViewer.gameObject.SetActive(true);
        bridgeDepartmentProductionViewer.Initialize();
    }

    private void EngineerProductivityPanelInit()
    {
        CloseAllPanels();
        if (engineeringDepartmentProductionViewer == null)
        {
            Debug.LogError("Engineering production viewer is null");
            return;
        }

        engineeringDepartmentProductionViewer.gameObject.SetActive(true);
        engineeringDepartmentProductionViewer.Initialize();
    }
    
    private void ScienceProductivityPanelInit()
    {
        CloseAllPanels();
        if (scienceDepartmentProductionViewer == null)
        {
            Debug.LogError("Science production viewer is null");
            return;
        }
        scienceDepartmentProductionViewer.gameObject.SetActive(true);
        scienceDepartmentProductionViewer.Initialize();
    }
    
    private void CargoProductivityPanelInit()
    {
        CloseAllPanels();
        if (cargoDepartmentProductionViewer == null)
        {
            Debug.LogError("Cargo production viewer is null");
            return;
        }
        cargoDepartmentProductionViewer.gameObject.SetActive(true);
        cargoDepartmentProductionViewer.Initialize();
    }
    
    private void BarProductivityPanelInit()
    {
        CloseAllPanels();
        if (barDepartmentProductionViewer == null)
        {
            Debug.LogError("Cargo production viewer is null");
            return;
        }
        barDepartmentProductionViewer.gameObject.SetActive(true);
        barDepartmentProductionViewer.Initialize();
    }

    private void CloseAllPanels()
    {
        bridgeDepartmentProductionViewer.gameObject.SetActive(false);
        engineeringDepartmentProductionViewer.gameObject.SetActive(false);
        scienceDepartmentProductionViewer.gameObject.SetActive(false);
        cargoDepartmentProductionViewer.gameObject.SetActive(false);
        // medbayDepartmentProductionViewer.gameObject.SetActive(false);
        // securityDepartmentProductionViewer.gameObject.SetActive(false);
        barDepartmentProductionViewer.gameObject.SetActive(false);
    }
    
}
