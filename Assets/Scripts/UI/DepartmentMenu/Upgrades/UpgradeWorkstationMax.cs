using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeWorkstationMax : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Department _department;

    private void Awake()
    {
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            UpgradeService upgradeService = ServiceLocator.Get<UpgradeService>();
            upgradeService.ApplyUpgrade(UpgradeDataSO.UpgradeType.DepartmentMaxWorkstations, _department);
        }).AddTo(this);
    }
}
