using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardMenuController : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private List<DailyRewardButton> rewardButtons; // Используем List для удобства
    [SerializeField] private Button closeButton;

    private DailyRewardService dailyRewardService;
    private DailyRewardsConfig rewardsConfig;

    private void Awake()
    {
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().DailyRewardHide();
        }).AddTo(this);
    }

    public void Show()
    {
        content.SetActive(true);
        Initialize();
    }

    private void Initialize()
    {
        dailyRewardService = ServiceLocator.Get<DailyRewardService>();
        rewardsConfig = ServiceLocator.Get<DataLibrary>().dailyRewardsConfig;

        if (dailyRewardService == null || rewardsConfig == null || rewardButtons.Count == 0)
        {
            Debug.LogError("DailyRewardService, DailyRewardsConfig или rewardButtons не инициализированы!");
            return;
        }

        var playerController = ServiceLocator.Get<PlayerController>();
        int claimedDays = playerController.PlayerData.dailyRewardClaimedDaysCount;
        int cycleLength = rewardsConfig.rewards.Count;

        for (int i = 0; i < rewardButtons.Count; i++)
        {
            int dayNumber = claimedDays + 1 + i; // Сегодня, завтра, послезавтра
            int rewardIndex = (dayNumber - 1) % cycleLength;

            if (rewardIndex >= 0 && rewardIndex < rewardsConfig.rewards.Count)
            {
                rewardButtons[i].Initialize(dayNumber, rewardsConfig.rewards[rewardIndex], dailyRewardService);
            }
            else
            {
                Debug.LogError($"Ошибка: Индекс награды для дня {dayNumber} за пределами диапазона.");
                rewardButtons[i].Initialize(dayNumber, new DailyRewardsConfig.Reward(), dailyRewardService); // Инициализируем с пустыми данными, чтобы избежать ошибок
            }
        }
    }

    public void Hide()
    {
        content.SetActive(false);
    }
}