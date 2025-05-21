using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WorldDepartmentButtonHandler : MonoBehaviour
{
    [SerializeField] private Department department;
    [SerializeField] private Button departmentMenuButton;
    [SerializeField] private Button hazardButton;
    
    private void Start()
    {
        departmentMenuButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().DepartmentScreenShow(department);   
        }).AddTo(this);
        
        hazardButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<StationEventsController>().MajorEventFinish(department);
        }).AddTo(this);
    }

    public void DepartmentButtonToggle(bool isOn)
    {
        departmentMenuButton.gameObject.SetActive(isOn);
    }

    public void HazardButtonToggle(bool isOn)
    {
        hazardButton.gameObject.SetActive(isOn);
        departmentMenuButton.gameObject.SetActive(!isOn);
    }
}
