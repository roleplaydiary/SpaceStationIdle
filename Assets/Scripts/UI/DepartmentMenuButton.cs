using UnityEngine;

public class DepartmentMenuButton : MonoBehaviour
{
    [SerializeField] private Department department;

    public void OnButtonClick()
    {
        ServiceLocator.Get<UIController>().DepartmentScreenShow(department);    
    }
}
