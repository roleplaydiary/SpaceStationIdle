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
    
    private void Start()
    {
        playerController = ServiceLocator.Get<PlayerController>(); // Получаем PlayerController
        if (playerController == null)
        {
            Debug.LogError("PlayerController не найден в ServiceLocator!");
            return;
        }
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
                var resourceManager = ServiceLocator.Get<ResourceManager>();
                resourceManager.SaveResources();
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
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
                creditsThisFrame += workBenchesList[i].ProductionRate * RESOURCE_UPDATE_INTERVAL / 60f; // Переводим в секунды
        }

        if (creditsThisFrame > 0)
        {
            playerController.PlayerData.playerCredits.Value += creditsThisFrame;
        }
    }
    
    public void ProduceRandomResource()
    {
        // if (workingCrew.Count < 4)// В карго 4 и 5 станок - станки шахтёров
        // {
        //     Debug.Log("Ресурс не производится, потому что никто не работает на 4 и 5 станке");
        //     return;
        // }
        
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
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        if (!IsStationEnergyEnough())
        {
            return result;
        }
        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            result += workBenchesList[i].ProductionRate;
        }
        
        return result;
    }
    
    public override void AddAFKProduction(System.TimeSpan afkTime)
    {
        float totalCreditsEarned = 0f;
        int workingCrewCount = workingCrew.Count;
        float productionRatePerMinutePerBench = 0f; // Нужно получить фактическую скорость производства верстака

        for (int i = 0; i < workingCrewCount; i++)
        {
            productionRatePerMinutePerBench += workBenchesList[i].ProductionRate;
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

    protected override void BenchesInitialization()
    {
        if (blockData.WorkBenchesInstalled == 0)
            return;

        if (workBenchesParent != null)
        {
            for (int i = 0; i < blockData.WorkBenchesInstalled && i < workBenchesParent.childCount; i++)
            {
                WorkBenchController workBenchController = workBenchesParent.GetChild(i).GetComponent<WorkBenchController>();
                if (workBenchController != null)
                {
                    workBenchesList.Add(workBenchController);
                    workBenchController.gameObject.SetActive(true);
                }
            }
        }
    }

    private void HireNewCrewMemberInternal()
    {
        base.HireNewCrewMember();
    }

    public override void HireNewCrewMember()
    {
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.maxCrew.Value)
        {
            base.HireNewCrewMember(); // Вызываем метод найма из родительского класса
            // Здесь можно добавить дополнительную логику для инженеров после найма, если необходимо
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
    }
}
