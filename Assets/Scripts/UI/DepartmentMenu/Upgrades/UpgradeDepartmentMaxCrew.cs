using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDepartmentMaxCrew : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Department _department;
    [SerializeField] private string upgradeId;

    private void Awake()
    {
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            UpgradeService upgradeService = ServiceLocator.Get<UpgradeService>();
            upgradeService.ApplyUpgrade(UpgradeDataSO.UpgradeType.DepartmentMaxCrew, _department);
        }).AddTo(this);
    }
}
