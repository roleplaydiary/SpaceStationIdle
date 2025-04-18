using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BridgeBlockController : StationBlockController
{
    // Здесь остаются только специфичные для мостика поля и методы
    private float creditsAccumulated = 0f;
    private float lastResourceUpdateTime = 0f;
    private const float RESOURCE_UPDATE_INTERVAL = 1f; // Обновление каждую секунду
    
    private PlayerData playerData;
    private PlayerController playerController;
    
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
    }
    
    private void Update()
    {
        if (Time.time - lastResourceUpdateTime >= RESOURCE_UPDATE_INTERVAL)
        {
            lastResourceUpdateTime = Time.time;
            ProduceCredits();
        }
    }
    
    private void ProduceCredits()
    {
        float creditsThisFrame = 0f;
        int workingCrewCount = workingCrew.Count;
        int workBenchesCount = workBenchesList.Count;

        if (playerData != null)
        {
            for (int i = 0; i < workingCrewCount && i < workBenchesCount; i++)
            {
                if (workBenchesList[i].ProducedResource == WorkBenchResource.Credits)
                {
                    creditsThisFrame += workBenchesList[i].ProductionRate * RESOURCE_UPDATE_INTERVAL / 60f; // Переводим в секунды
                }
            }

            if (creditsThisFrame > 0)
            {
                // Обновляем значение кредитов в *том же самом* экземпляре PlayerData
                playerController.PlayerData.playerCredits.Value += Mathf.RoundToInt(creditsThisFrame);
                // Здесь можно вызвать событие обновления UI кредитов
            }
        }
    }

    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        // Дополнительная инициализация для мостика, если нужна
    }

    protected override void BenchesInitialization()
    {
        if (blockData.WorkBenchesLevelUnlocked == 0)
            return;

        if (workBenchesParent != null)
        {
            for (int i = 0; i < blockData.WorkBenchesLevelUnlocked && i < workBenchesParent.childCount; i++)
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

    protected override void CrewInitialization()
    {
        base.CrewInitialization();
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

    public override void UnlockWorkBench()
    {
        base.UnlockWorkBench();
    }

    protected override Vector3 GetAvailableIdlePosition()
    {
        if (idlePositionList.Count > idleCrew.Count)
        {
            return idlePositionList[idleCrew.Count].position;
        }
        return transform.position; // В качестве запасного варианта
    }

    protected override void InitializeLists()
    {
        // Вызываем базовую реализацию для обработки idlePositionParent
        base.InitializeLists();

        // Дополнительная инициализация списков, специфичная для мостика
        if (workBenchesParent != null)
        {
            for (int i = 0; i < workBenchesParent.childCount; i++)
            {
                WorkBenchController workBenchController = workBenchesParent.GetChild(i).GetComponent<WorkBenchController>();
                if (workBenchController != null)
                {
                    workBenchesList.Add(workBenchController);
                }
            }
        }
    }
}