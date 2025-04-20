using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DebugButtonsController : MonoBehaviour
{
    [SerializeField]
        private Button upgradeWorkbenchesMaxEngineering;
    [SerializeField]
        private Button addWorkbenchEngineering;
    
    private CompositeDisposable _disposables = new CompositeDisposable();

    private void Start()
    {
        if (upgradeWorkbenchesMaxEngineering)
        {
            upgradeWorkbenchesMaxEngineering.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    StationController station = ServiceLocator.Get<StationController>();
                    station.UpgradeDepartmentWorkbenchesMax(Department.Engineer);
                    Debug.Log("Upgrade Engineering workbenches max");
                }).AddTo(_disposables);
        }

        if (addWorkbenchEngineering)
        {
            addWorkbenchEngineering.OnClickAsObservable()
                .Subscribe(_ => 
            {
                StationController station = ServiceLocator.Get<StationController>();
                station.UpgradeDepartmentWorkbenches(Department.Engineer);
                Debug.Log("Add Workbenches to engineering");
            }).AddTo(_disposables);
        }
    }
}
