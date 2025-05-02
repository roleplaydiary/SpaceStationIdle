using UnityEngine;
using UniRx;
using System;
using System.Threading.Tasks;

public class DailyRewardService
{
    private readonly DailyRewardsConfig rewardsConfig;
    private readonly ResourceManager resourceManager;
    private readonly ReactiveProperty<bool> canClaimReward = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> CanClaimReward => canClaimReward;
    private PlayerController playerController;

    public DailyRewardService()
    {
        rewardsConfig = ServiceLocator.Get<DataLibrary>().dailyRewardsConfig;
        resourceManager = ServiceLocator.Get<ResourceManager>();
        playerController = ServiceLocator.Get<PlayerController>();
        
        CheckIfRewardCanBeClaimed();
    }

    public void CheckIfRewardCanBeClaimed()
    {
        DateTime? lastClaimedDate = playerController.GetPlayerData().lastDailyRewardClaimedDate;
        Debug.Log($"DailyRewardService: lastClaimedDate = {lastClaimedDate}");

        DateTime today = DateTime.Now.Date;
        DateTime yesterday = today.AddDays(-1);

        if (!lastClaimedDate.HasValue)
        {
            Debug.Log("DailyRewardService: Первая награда доступна");
            canClaimReward.Value = true;
            return;
        }

        DateTime lastDate = lastClaimedDate.Value.Date;

        if (lastDate == today)
        {
            Debug.Log("DailyRewardService: Награда уже получена сегодня");
            canClaimReward.Value = false;
        }
        else if (lastDate == yesterday)
        {
            Debug.Log("DailyRewardService: Продолжаем цепочку наград");
            canClaimReward.Value = true;
        }
        else if (lastDate < yesterday)
        {
            Debug.Log("DailyRewardService: Цепочка сброшена, начинаем заново");
            playerController.GetPlayerData().dailyRewardClaimedDaysCount = 0;
            canClaimReward.Value = true;
        }
        else
        {
            Debug.LogWarning("DailyRewardService: Некорректная дата в данных игрока");
            canClaimReward.Value = false;
        }
    }


    public async Task ClaimReward()
    {
        if (!canClaimReward.Value || resourceManager == null)
        {
            Debug.Log($"Награда на сегодня уже получена или сервисы/данные не инициализированы.");
            return;
        }

        int rewardIndex = playerController.GetPlayerData().dailyRewardClaimedDaysCount % rewardsConfig.rewards.Count;

        if (rewardIndex >= 0 && rewardIndex < rewardsConfig.rewards.Count)
        {
            DailyRewardsConfig.Reward reward = rewardsConfig.rewards[rewardIndex];

            if (reward.type == DailyRewardsConfig.Reward.RewardType.Credits)
            {
                playerController.AddCredits(reward.creditAmount);
                Debug.Log($"Получена ежедневная награда: {reward.creditAmount} кредитов (день {playerController.GetPlayerData().dailyRewardClaimedDaysCount + 1}).");
            }
            else if (reward.type == DailyRewardsConfig.Reward.RewardType.Resource && reward.resourceReward != null)
            {
                foreach (var resourceKvp in reward.resourceReward)
                {
                    if (Enum.TryParse<ResourceType>(resourceKvp.Key, out var resourceType))
                    {
                        resourceManager.AddResource(resourceType, resourceKvp.Value);
                        Debug.Log($"Получена еженедельная награда (день {playerController.GetPlayerData().dailyRewardClaimedDaysCount + 1}): {resourceKvp.Key} x{resourceKvp.Value}.");
                    }
                    else
                    {
                        Debug.LogError($"Неизвестный тип ресурса в конфигурации: {resourceKvp.Key}");
                    }
                }
            }

            // Обновляем данные игрока
            playerController.UpdateDailyRewardData();

            // Сохраняем обновленные данные игрока
            await playerController.SavePlayerData();
            CheckIfRewardCanBeClaimed();
        }
        else
        {
            Debug.LogError("Ошибка: Индекс награды за пределами диапазона.");
        }
    }
}