using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class CargoBlockController : StationBlockController
{
    // Здесь остаются только специфичные для карго поля и методы
    private float creditsAccumulated = 0f;
    private float lastResourceUpdateTime = 0f;
    private const float RESOURCE_UPDATE_INTERVAL = 1f; // Обновление каждую секунду
    public static float RESOURCE_DROP_INTERVAL = 600; // Обновление каждую секунду
    
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
        
        Observable.Interval(System.TimeSpan.FromSeconds(RESOURCE_DROP_INTERVAL))
            .Where(_ => isProductionOn.Value)
            .Subscribe(_ =>
            {
                ProduceRandomResource();
            })
            .AddTo(disposables);
    }
    
    private void OnDestroy()
    {
        disposables.Clear();
    }
    
    private void ProduceCreditsReactive()
    {
        float creditsThisFrame = 0f;
        int workingCrewCount = crewManager.workingCrew.Count;
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
    
    public void ProduceRandomResource()
    {
        var dataLibrary = ServiceLocator.Get<DataLibrary>();
        if (dataLibrary == null || dataLibrary.resourceDropData == null)
        {
            Debug.LogError("CargoResourceProductionDataSO не найден в DataLibrary!");
            return;
        }

        CargoResourceProductionDataSO resourceData = dataLibrary.resourceDropData;
        var resourceManager = ServiceLocator.Get<ResourceManager>();
        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager не найден!");
            return;
        }

        foreach (var entry in resourceData.possibleResources)
        {
            if (Random.Range(0f, 1f) <= entry.dropProbability)
            {
                float amount = Random.Range(entry.minAmount, entry.maxAmount);
                if (amount > 0)
                {
                    resourceManager.AddResource(entry.resource, amount);
                    Debug.Log($"Карго отдел обнаружил {amount:F2} ед. ресурса '{entry.resource}'.");
                }
            }
        }
    }

    public override float GetProductionValue()
    {
        float result = 0f;
        int workingCrewCount = crewManager.workingCrew.Count;
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
        int workingCrewCount = crewManager.workingCrew.Count;
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
                Debug.Log($"Карго отдел произвел {(int)totalCreditsEarned} кредитов за время отсутствия.");
            }
        }
        else
        {
            Debug.Log($"Производство кредитов в Карго остановлено из-за нехватки энергии во время отсутствия.");
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
