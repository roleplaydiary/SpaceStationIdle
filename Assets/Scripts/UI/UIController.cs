using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private DepartmentMenuViewer departmentMenuViewer;
    
    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void LoadingScreenShow()
    {
        loadingScreen.SetActive(true);
    }
    
    public void LoadingScreenHide()
    {
        loadingScreen.SetActive(false);
    }

    public void DepartmentScreenShow(Department department)
    {
        departmentMenuViewer.Show(department);
    }
    
    public void DepartmentScreenHide()
    {
        departmentMenuViewer.Hide();        
    }
}
