using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStationMaxCrew : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Awake()
    {
        _button.OnClickAsObservable().Subscribe(_ =>
        {
            UpgradeService upgradeService = ServiceLocator.Get<UpgradeService>();
            upgradeService.ApplyUpgrade(UpgradeDataSO.UpgradeType.StationMaxCrew);
        }).AddTo(this);
    }
}
