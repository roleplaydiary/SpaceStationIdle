using UnityEngine;

public class DepartmentProductionPanelController : MonoBehaviour
{
    [SerializeField] private DepartmentProductionViewer bridgeDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer engineeringDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer rndDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer cargoDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer medbayDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer securityDepartmentProductionViewer;
    [SerializeField] private DepartmentProductionViewer kitchenDepartmentProductionViewer;

    public void ProductivityPanelInit(Department department)
    {
        switch (department)
        {
            case Department.Bridge:
            {
                BridgeProductivityPanelInit();
                break;
            }

            case Department.Engineer:
            {
                EngineerProductivityPanelInit();
                break;
            }
        }
    }
    
    private void BridgeProductivityPanelInit()
    {
        CloseAllPanels();
        bridgeDepartmentProductionViewer.gameObject.SetActive(true);
        bridgeDepartmentProductionViewer.Initialize();
    }

    private void EngineerProductivityPanelInit()
    {
        CloseAllPanels();
        engineeringDepartmentProductionViewer.gameObject.SetActive(true);
        engineeringDepartmentProductionViewer.Initialize();
    }

    private void CloseAllPanels()
    {
        bridgeDepartmentProductionViewer.gameObject.SetActive(false);
        engineeringDepartmentProductionViewer.gameObject.SetActive(false);
        // rndDepartmentProductionViewer.gameObject.SetActive(false);
        // cargoDepartmentProductionViewer.gameObject.SetActive(false);
        // medbayDepartmentProductionViewer.gameObject.SetActive(false);
        // securityDepartmentProductionViewer.gameObject.SetActive(false);
        // kitchenDepartmentProductionViewer.gameObject.SetActive(false);
    }
    
}
