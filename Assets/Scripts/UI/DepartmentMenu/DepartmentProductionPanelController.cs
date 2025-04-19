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
        bridgeDepartmentProductionViewer.gameObject.SetActive(true);
        bridgeDepartmentProductionViewer.Initialize();
    }

    private void EngineerProductivityPanelInit()
    {
        engineeringDepartmentProductionViewer.gameObject.SetActive(true);
        engineeringDepartmentProductionViewer.Initialize();
    }
}
