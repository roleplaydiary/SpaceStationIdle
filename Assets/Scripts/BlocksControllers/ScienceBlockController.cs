using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ScienceBlockController : StationBlockController
{
    // Здесь остаются только специфичные для мостика поля и методы
    private float creditsAccumulated = 0f;
    private float lastResourceUpdateTime = 0f;
    private const float RESOURCE_UPDATE_INTERVAL = 1f; // Обновление каждую секунду
    
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

        // Реактивный поток для производства Ресерч поинтов
        Observable.Interval(System.TimeSpan.FromSeconds(RESOURCE_UPDATE_INTERVAL))
            .Where(_ => isProductionOn.Value && playerData != null)
            .Subscribe(_ => ProduceResearchPointsReactive())
            .AddTo(disposables);
    }
    
    private void OnDestroy()
    {
        disposables.Clear();
    }
    
    private void ProduceResearchPointsReactive()
    {
        float rpThisFrame = 0f;
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
        {
            rpThisFrame += workBenchesList[i].ProductionRate * RESOURCE_UPDATE_INTERVAL / 60f; // Переводим в секунды
        }

        if (rpThisFrame > 0)
        {
            playerController.PlayerData.researchPoints.Value += rpThisFrame;
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
        float totalResearchPointsEarned = 0f;
        int workingCrewCount = workingCrew.Count;
        float productionRatePerMinutePerBench = 0f; // Нужно получить фактическую скорость производства верстака

        for (int i = 0; i < workingCrewCount; i++)
        {
            productionRatePerMinutePerBench += workBenchesList[i].ProductionRate;
        }

        // Учитываем только работающий экипаж и доступную энергию
        if (IsStationEnergyEnough())
        {
            totalResearchPointsEarned = productionRatePerMinutePerBench * workingCrewCount * (float)afkTime.TotalMinutes;
            if (totalResearchPointsEarned > 0)
            {
                ServiceLocator.Get<PlayerController>().AddResearchPoints(totalResearchPointsEarned);
                Debug.Log($"РНД отдел произвел {(int)totalResearchPointsEarned} очков исследований за время отсутствия.");
            }
        }
        else
        {
            Debug.Log($"Производство очков исследований в РНД остановлено из-за нехватки энергии во время отсутствия.");
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
            base.HireNewCrewMember();
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
    }
}
