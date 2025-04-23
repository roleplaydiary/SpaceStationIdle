using UnityEngine;
using System;
using System.Threading.Tasks;

public class AFKController : MonoBehaviour
{
    private const string AFKTimeLimitKey = "AFKTimeLimit";

    private void Awake()
    {
        ServiceLocator.Register(this); // Регистрируем контроллер при создании
    }

    public async Task CheckAFKProduction()
    {
        PlayerController playerController = ServiceLocator.Get<PlayerController>();
        if (playerController == null || playerController.PlayerData == null)
        {
            Debug.LogError("PlayerController или PlayerData не найдены!");
            return;
        }

        DateTime lastSaveTimeUtc = playerController.GetLastSaveTime();
        DateTime nowUtc = DateTime.UtcNow;

        TimeSpan afkTime = nowUtc - lastSaveTimeUtc;
        Debug.Log($"Время отсутствия: {afkTime}");

        float afkTimeLimitHours = GetAFKTimeLimit();
        TimeSpan limitedAFKTime = afkTime;
        if (afkTime.TotalHours > afkTimeLimitHours)
        {
            limitedAFKTime = TimeSpan.FromHours(afkTimeLimitHours);
            Debug.Log($"Производство за время отсутствия ограничено до {limitedAFKTime}");
        }

        await CalculateDepartmentProduction(limitedAFKTime);

        // Обновляем время последнего сохранения после расчета АФК
        playerController.PlayerData.lastSaveTime = nowUtc.ToLocalTime(); // Сохраняем локальное время
    }

    private async Task CalculateDepartmentProduction(TimeSpan afkTime)
    {
        var stationController = ServiceLocator.Get<StationController>();
        if (stationController != null && stationController.StationBlocks != null && stationController.StationData != null)
        {
            foreach (var blockController in stationController.StationBlocks)
            {
                Department blockType = blockController.GetBlockType();
                if (stationController.StationData.IsUnlocked(blockType))
                {
                    blockController.AddAFKProduction(afkTime);

                    // Добавляем логику для имитации добычи ресурсов в карго
                    if (blockController is CargoBlockController cargoController)
                    {
                        float afkTimeInMinutes = (float)afkTime.TotalMinutes;
                        float dropIntervalMinutes = CargoBlockController.RESOURCE_DROP_INTERVAL / 60f;
                        int numberOfResourceDrops = Mathf.FloorToInt(afkTimeInMinutes / dropIntervalMinutes);

                        Debug.Log($"Имитация добычи ресурсов в карго за {afkTimeInMinutes:F2} минут (попыток: {numberOfResourceDrops}).");

                        for (int i = 0; i < numberOfResourceDrops; i++)
                        {
                            cargoController.ProduceRandomResource();
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("StationController или StationBlocks или StationData не найдены!");
        }

        // Сохраняем ресурсы после обработки всего АФК
        var resourceManager = ServiceLocator.Get<ResourceManager>();
        if (resourceManager != null)
        {
            await resourceManager.SaveResources();
        }
    }

    private float GetAFKTimeLimit()
    {
        DataLibrary dataLibrary = ServiceLocator.Get<DataLibrary>();
        PlayerController playerController = ServiceLocator.Get<PlayerController>();

        if (dataLibrary != null && dataLibrary.donateData != null && playerController != null && playerController.PlayerData != null)
        {
            int currentAFKLevel = playerController.PlayerData.afkLevel.Value;

            // По умолчанию 1 час
            float afkLimitHours = 1f;

            // Ищем апгрейд AFKTime с соответствующим уровнем
            foreach (var upgrade in dataLibrary.donateData.upgrades)
            {
                if (upgrade.type == DonateUpgradesSO.UpgradeType.AFKTime && upgrade.level == currentAFKLevel)
                {
                    afkLimitHours = upgrade.value;
                    break;
                }
            }
            return afkLimitHours;
        }
        else
        {
            Debug.LogError("DataLibrary, DonateData или PlayerController не найдены при получении лимита АФК.");
            return 1f;
        }
    }

    public void SetAFKLevel(int level)
    {
        PlayerController playerController = ServiceLocator.Get<PlayerController>();
        if (playerController != null && playerController.PlayerData != null)
        {
            playerController.PlayerData.afkLevel.Value = level;
            playerController.SavePlayerData(); // Сохраняем уровень доната
            Debug.Log($"Уровень АФК доната установлен на {level}.");
        }
        else
        {
            Debug.LogError("PlayerController не найден при установке уровня АФК доната.");
        }
    }
}