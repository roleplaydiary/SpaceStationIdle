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
    private PlayerData playerData;

    public DailyRewardService()
    {
        rewardsConfig = ServiceLocator.Get<DataLibrary>().dailyRewardsConfig;
        resourceManager = ServiceLocator.Get<ResourceManager>();
        playerController = ServiceLocator.Get<PlayerController>();
        playerData = playerController.GetPlayerData();
        CheckIfRewardCanBeClaimed();
    }

    public void CheckIfRewardCanBeClaimed()
    {
        if (playerData != null && (!playerData.lastDailyRewardClaimedDate.HasValue || playerData.lastDailyRewardClaimedDate.Value.Date < DateTime.Now.Date))
        {
            canClaimReward.Value = true;
        }
        else
        {
            canClaimReward.Value = false;
        }
    }

    public async Task ClaimReward()
    {
        if (!canClaimReward.Value || resourceManager == null || playerData == null)
        {
            Debug.Log($"Награда на сегодня уже получена или сервисы/данные не инициализированы.");
            return;
        }

        int rewardIndex = playerData.dailyRewardClaimedDaysCount % rewardsConfig.rewards.Count;

        if (rewardIndex >= 0 && rewardIndex < rewardsConfig.rewards.Count)
        {
            DailyRewardsConfig.Reward reward = rewardsConfig.rewards[rewardIndex];

            if (reward.type == DailyRewardsConfig.Reward.RewardType.Credits)
            {
                playerController.AddCredits(reward.creditAmount);
                Debug.Log($"Получена ежедневная награда: {reward.creditAmount} кредитов (день {playerData.dailyRewardClaimedDaysCount + 1}).");
            }
            else if (reward.type == DailyRewardsConfig.Reward.RewardType.Resource && reward.resourceReward != null)
            {
                foreach (var resourceKvp in reward.resourceReward)
                {
                    if (Enum.TryParse<ResourceType>(resourceKvp.Key, out var resourceType))
                    {
                        resourceManager.AddResource(resourceType, resourceKvp.Value);
                        Debug.Log($"Получена еженедельная награда (день {playerData.dailyRewardClaimedDaysCount + 1}): {resourceKvp.Key} x{resourceKvp.Value}.");
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
            canClaimReward.Value = false;
        }
        else
        {
            Debug.LogError("Ошибка: Индекс награды за пределами диапазона.");
        }
    }

    // Метод для загрузки данных о ежедневных наградах при инициализации
    public void LoadDailyRewardData(PlayerData loadedPlayerData)
    {
        if (loadedPlayerData != null)
        {
            playerData.lastDailyRewardClaimedDate = loadedPlayerData.lastDailyRewardClaimedDate;
            playerData.dailyRewardClaimedDaysCount = loadedPlayerData.dailyRewardClaimedDaysCount;
        }
        else
        {
            playerData.lastDailyRewardClaimedDate = null;
            playerData.dailyRewardClaimedDaysCount = 0;
        }
        
        CheckIfRewardCanBeClaimed();
    }
}