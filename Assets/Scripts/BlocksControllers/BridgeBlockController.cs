using UniRx;
using UnityEngine;

public class BridgeBlockController : StationBlockController
{
    // Здесь остаются только специфичные для мостика поля и методы
    private float creditsAccumulated = 0f;
    private float lastResourceUpdateTime = 0f;
    private const float RESOURCE_UPDATE_INTERVAL = 1f; // Обновление каждую секунду
    
    private PlayerData playerData;
    private PlayerController playerController;
    
    private ReactiveProperty<bool> isProductionOn = new ReactiveProperty<bool>(false);
    private CompositeDisposable disposables = new CompositeDisposable();
    
    
    
    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        playerController = ServiceLocator.Get<PlayerController>();
        playerData = playerController.PlayerData; // Получаем ссылку на PlayerData через контроллер
        if (playerData == null)
        {
            Debug.LogError("PlayerData не найден через PlayerController!");
            return;
        }

        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        energyService.CurrentStationEnergy
            .Subscribe(value => isProductionOn.Value = value > 0)
            .AddTo(disposables);

        // Реактивный поток для производства кредитов
        Observable.Interval(System.TimeSpan.FromSeconds(RESOURCE_UPDATE_INTERVAL))
            .Where(_ => isProductionOn.Value && playerData != null)
            .Subscribe(_ => ProduceCreditsReactive())
            .AddTo(disposables);
    }
    
    private void OnDestroy()
    {
        disposables.Clear();
    }
    
    private void ProduceCreditsReactive()
    {
        float creditsThisFrame = 0f;
        int workingCrewCount = CrewManager.workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
                creditsThisFrame += workBenchesList[i].GetProductionRate() * RESOURCE_UPDATE_INTERVAL / 60f; // Переводим в секунды
        }

        if (creditsThisFrame > 0)
        {
            playerController.PlayerData.playerCredits.Value += creditsThisFrame;
        }
    }

    public override float GetProductionValue()
    {
        float result = 0f;
        int workingCrewCount = CrewManager.workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        if (!IsStationEnergyEnough())
        {
            return result;
        }
        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            result += workBenchesList[i].GetProductionRate();
        }
        
        return result;
    }
    
    public override void AddAFKProduction(System.TimeSpan afkTime)
    {
        float totalCreditsEarned = 0f;
        int workingCrewCount = CrewManager.workingCrew.Count;
        float productionRatePerMinutePerBench = 0f; // Нужно получить фактическую скорость производства верстака

        for (int i = 0; i < workingCrewCount; i++)
        {
            productionRatePerMinutePerBench += workBenchesList[i].GetProductionRate();
        }

        // Учитываем только работающий экипаж и доступную энергию
        if (IsStationEnergyEnough())
        {
            totalCreditsEarned = productionRatePerMinutePerBench * workingCrewCount * (float)afkTime.TotalMinutes;
            if (totalCreditsEarned > 0)
            {
                ServiceLocator.Get<PlayerController>().AddCredits(totalCreditsEarned);
                Debug.Log($"Мостик произвел {(int)totalCreditsEarned} кредитов за время отсутствия.");
            }
        }
        else
        {
            Debug.Log($"Производство кредитов на мостике остановлено из-за нехватки энергии во время отсутствия.");
        }
    }

    private bool IsStationEnergyEnough()
    {
        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        if (energyService != null && energyService.CurrentStationEnergy.Value < 0)
        {
            Debug.Log("Недостаточно энергии для производства в " + name);
            return false;
        }
        return true;
    }
}