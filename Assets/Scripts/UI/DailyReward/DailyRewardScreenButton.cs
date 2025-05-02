using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardScreenButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject backgroundActive;
    [SerializeField] private GameObject backgroundInactive;
    private void Start()
    {
        button.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().DailyRewardShow();
        }).AddTo(this);
        
        var gameController = ServiceLocator.Get<GameController>();
        gameController.OnGameInitialized
            .Where(initialized => initialized)
            .Subscribe(_ => ButtonStateInitialize())
            .AddTo(this);
    }

    private void ButtonStateInitialize()
    {
        var dailyRewardService = ServiceLocator.Get<DailyRewardService>();
        dailyRewardService.CanClaimReward.Subscribe(value =>
        {
            backgroundActive.SetActive(value);
            backgroundInactive.SetActive(!value);
        }).AddTo(this);

        var canClaim = dailyRewardService.CanClaimReward.Value;
        backgroundActive.SetActive(canClaim);
        backgroundInactive.SetActive(!canClaim);
    }
}
