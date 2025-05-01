using Controllers;
using Services;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DepartmentMenuViewer : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private DepartmentInfoPanelViewer departmentInfoPanelViewer;
    [SerializeField] private CrewAssignmentPanelController crewAssignmentPanelController;
    [SerializeField] private DepartmentProductionPanelController departmentProductionPanelController;
    [SerializeField] private DepartmentUpgradesScrollController departmentUpgradesScrollController;
    [SerializeField] private TMP_Text departmentName;
    [SerializeField] private Button closeButton;
    
    private CompositeDisposable disposables;

    private void Awake()
    {
        ServiceLocator.Register(this);

        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().DepartmentScreenHide();
        }).AddTo(this);
    }

    public void Show(Department department)
    {
        content.SetActive(true);
        
        var crewService = ServiceLocator.Get<CrewService>();

        disposables?.Clear();
        disposables = new CompositeDisposable();
        Observable.Merge(crewService.OnWorkingCrewValueUpdate, crewService.OnRestingCrewValueUpdate)
            .Subscribe(value =>
            {
               departmentInfoPanelViewer.Initialization(department);
               departmentProductionPanelController.ProductivityPanelInit(department);
            })
            .AddTo(disposables);
        
        Initialize(department);
    }

    public void Hide()
    {
        disposables.Dispose();
        content.SetActive(false);
    }

    private void Initialize(Department department)
    {
        departmentName.text = department.ToString();

        departmentInfoPanelViewer.Initialization(department);

        StationBlockController blockController = null;
        var stationController = ServiceLocator.Get<StationController>();
        if (stationController != null && stationController.StationBlocks != null)
        {
            blockController = stationController.StationBlocks.Find(block => block.GetBlockType() == department);
        }

        crewAssignmentPanelController.Initialize(blockController);
        departmentProductionPanelController.ProductivityPanelInit(department);
        departmentUpgradesScrollController.Initialize(department);
    }
}