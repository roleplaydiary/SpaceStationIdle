using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BuyUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Department _department;
    [SerializeField] private string upgradeId;

    private void Awake()
    {
        if (upgradeId == null)
        {
            Debug.LogError($"{nameof(upgradeId)} is null");
            return;
        }
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            UpgradeService upgradeService = ServiceLocator.Get<UpgradeService>();
            upgradeService.PurchaseUpgrade(upgradeId, _department);
        }).AddTo(this);
    }
}
