using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonsController : MonoBehaviour
{
    [SerializeField]
        private Button upgradeWorkbenchesMaxEngineering;
    [SerializeField]
        private Button addWorkbenchEngineering;

    [SerializeField] private Button unlockRNDButton;
    [SerializeField] private Button upgradeMaxCrewRndButton;
    [SerializeField] private Button hireScientistButton;
    [SerializeField] private Button upgradeMaxWorkstationRNDButton;
    [SerializeField] private Button addWorkstationRNDButton;

    [SerializeField] private Button saveResources;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {
        if (upgradeWorkbenchesMaxEngineering)
        {
            upgradeWorkbenchesMaxEngineering.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    StationController station = ServiceLocator.Get<StationController>();
                    station.UpgradeDepartmentWorkbenchesMax(Department.Engineering);
                    Debug.Log("Upgrade Engineering workbenches max");
                }).AddTo(_disposables);
        }

        if (addWorkbenchEngineering)
        {
            addWorkbenchEngineering.OnClickAsObservable()
                .Subscribe(_ => 
            {
                StationController station = ServiceLocator.Get<StationController>();
                station.UpgradeDepartmentWorkbenches(Department.Engineering);
                Debug.Log("Add Workbenches to engineering");
            }).AddTo(_disposables);
        }

        unlockRNDButton.OnClickAsObservable().Subscribe(button =>
        {
            UnlockRND();
        }).AddTo(_disposables);

        upgradeMaxCrewRndButton.OnClickAsObservable().Subscribe(button =>
        {
            UpgradeMaxCrewRND();
        }).AddTo(_disposables);
        
        hireScientistButton.OnClickAsObservable().Subscribe(button =>
        {
            HireScientist();
        }).AddTo(_disposables);
        
        upgradeMaxWorkstationRNDButton.OnClickAsObservable().Subscribe(button =>
        {
            UpgradeMaxWorkStationsRND();
        }).AddTo(_disposables);
        
        addWorkstationRNDButton.OnClickAsObservable().Subscribe(button =>
        {
            AddWorkstationRND();
        }).AddTo(_disposables);

        saveResources.OnClickAsObservable().Subscribe(async button =>
        {
            ResourceManager resourceManager = ServiceLocator.Get<ResourceManager>();
            await ServiceLocator.Get<CloudController>().SaveResources(resourceManager.GetCurrentResources());
        }).AddTo(_disposables);
    }

    public void UnlockRND()
    {
        ServiceLocator.Get<StationController>().UnlockStationBlock(Department.Science);
    }

    public void UpgradeMaxCrewRND()
    {
        ServiceLocator.Get<StationController>().UpgradeDepartmentMaxCrew(Department.Science);
    }

    public void HireScientist()
    {
        ServiceLocator.Get<StationController>().HireCrewMember(Department.Science);
    }

    public void UpgradeMaxWorkStationsRND()
    {
        ServiceLocator.Get<StationController>().UpgradeDepartmentWorkbenchesMax(Department.Science);
    }

    public void AddWorkstationRND()
    {
        ServiceLocator.Get<StationController>().UpgradeDepartmentWorkbenches(Department.Science);
    }
}
