using UnityEngine;

public class DepartmentMenuButton : MonoBehaviour
{
    [SerializeField] private Department department;
    [SerializeField] private DepartmentMenuViewer departmentMenuViewer;

    public void OnButtonClick()
    {
        departmentMenuViewer.Show(department);
    }
}
