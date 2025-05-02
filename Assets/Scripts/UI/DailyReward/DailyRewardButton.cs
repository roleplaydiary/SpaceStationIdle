using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text rewardTitle;
    [SerializeField] private TMP_Text rewardLabel;
    [SerializeField] private GameObject backgroundActive;
    [SerializeField] private GameObject backgroundInactive;

    private int rewardIndex;
    private DailyRewardService dailyRewardService;

    private void Awake()
    {
        button.OnClickAsObservable().Subscribe(_ =>
        {
            GetReward();
        }).AddTo(this);
    }

    public void Initialize(int dayNumber, DailyRewardsConfig.Reward reward, DailyRewardService service)
    {
        dailyRewardService = service;
        rewardIndex = dayNumber - 1; // Индекс в списке наград (начиная с 0)
        rewardTitle.text = $"Day {dayNumber}";

        if (reward.type == DailyRewardsConfig.Reward.RewardType.Credits)
        {
            rewardLabel.text = $"Credits +{reward.creditAmount}";
        }
        else if (reward.type == DailyRewardsConfig.Reward.RewardType.Resource && reward.resourceReward != null)
        {
            List<string> resourceStrings = new List<string>();
            if (reward.resourceReward.Metal > 0)
            {
                resourceStrings.Add($"Metal x{reward.resourceReward.Metal}");
            }
            if (reward.resourceReward.Glass > 0)
            {
                resourceStrings.Add($"Glass x{reward.resourceReward.Glass}");
            }
            if (reward.resourceReward.Plastic > 0)
            {
                resourceStrings.Add($"Plastic x{reward.resourceReward.Plastic}");
            }
            if (reward.resourceReward.Phoron > 0)
            {
                resourceStrings.Add($"Phoron x{reward.resourceReward.Phoron}");
            }
            // Добавьте здесь проверки для других типов ресурсов, если они есть в вашем классе Resources

            rewardLabel.text = string.Join(", ", resourceStrings);
            if (string.IsNullOrEmpty(rewardLabel.text))
            {
                rewardLabel.text = "N/A"; // Если вдруг награда ресурсами, но их нет
            }
        }

        UpdateState();
    }

    private void UpdateState()
    {
        if (dailyRewardService != null)
        {
            var playerController = ServiceLocator.Get<PlayerController>();
            if (playerController != null && playerController.PlayerData != null)
            {
                int claimedDays = playerController.PlayerData.dailyRewardClaimedDaysCount;
                if (rewardIndex < claimedDays)
                {
                    // Награда за этот день уже получена
                    backgroundActive.SetActive(false);
                    backgroundInactive.SetActive(true);
                    button.interactable = false;
                }
                else if (rewardIndex == claimedDays && dailyRewardService.CanClaimReward.Value)
                {
                    // Сегодняшняя награда, доступна для получения
                    backgroundActive.SetActive(true);
                    backgroundInactive.SetActive(false);
                    button.interactable = true;
                }
                else
                {
                    // Будущая награда
                    backgroundActive.SetActive(false);
                    backgroundInactive.SetActive(true);
                    button.interactable = false;
                }
            }
        }
    }

    private async void GetReward()
    {
        var playerController = ServiceLocator.Get<PlayerController>();
        var claimedDays = playerController.PlayerData.dailyRewardClaimedDaysCount;
        if (dailyRewardService != null && dailyRewardService.CanClaimReward.Value && rewardIndex == claimedDays)
        {
            await dailyRewardService.ClaimReward();
            PlaySound();
            UpdateState();
        }
        else
        {
            Debug.Log("Невозможно получить награду.");
        }
    }

    private void PlaySound()
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        var dataLibrary = ServiceLocator.Get<DataLibrary>();
        audioManager.PlayUISound(dataLibrary.soundLibrary.GetUISound("reward_obtain"));
    }
}