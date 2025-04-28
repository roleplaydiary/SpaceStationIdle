using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BuyUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private UpgradeButtonViewer upgradeButtonViewer;
    [SerializeField] protected Department _department;
    [SerializeField] protected string upgradeId;

    protected virtual void Start()
    {
        if (upgradeId == null)
        {
            Debug.LogError($"{nameof(upgradeId)} is null");
            return;
        }
        UpgradeService upgradeService = ServiceLocator.Get<UpgradeService>();
        
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            upgradeService.PurchaseUpgrade(upgradeId, _department);
        }).AddTo(this);
        
        upgradeService.OnUpgradePurchased.Subscribe(_ =>
        {
            Initialize();
        }).AddTo(this);
        
        upgradeButtonViewer.Initialize(upgradeId);
        Initialize();
    }
    
    protected virtual void Initialize(){}
    protected virtual void ButtonEnableUpdate(){} // TODO: Имплементировать, в зависимости от того, хватает ли ресурсов
}
