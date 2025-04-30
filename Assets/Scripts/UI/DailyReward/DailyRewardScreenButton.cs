using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardScreenButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject backgroundActive;
    [SerializeField] private GameObject backgroundInactive;
    private void Awake()
    {
        button.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().DailyRewardShow();
        }).AddTo(this);
        
        //ButtonStateInitialize();
    }

    private void ButtonStateInitialize()
    {
        ServiceLocator.Get<DailyRewardService>().CanClaimReward.Subscribe(value =>
        {
            backgroundActive.SetActive(value);
            backgroundInactive.SetActive(!value);
        }).AddTo(this);
    }
}
