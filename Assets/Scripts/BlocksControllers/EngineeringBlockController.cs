using UniRx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EngineeringBlockController : StationBlockController
{
    // Здесь остаются только специфичные для инженерного блока поля и методы

    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        // Дополнительная инициализация для инженерного блока, если нужна
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
        if (blockData.MaxCrewUnlocked == 0 || blockData.CurrentCrewHired == 0)
            return;

        for (int i = 0; i < blockData.CurrentCrewHired; i++)
        {
            var newCrewMember = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[1], transform); // Предположим, что у инженеров другой префаб (индекс 1)
            CharacterController crewController = newCrewMember.GetComponent<CharacterController>();
            crewMembers.Add(crewController);
            allCrewMembers.Add(crewController);
        }
    }

    private void HireNewCrewMemberInternal()
    {
        var newCrewMember = Instantiate(ServiceLocator.Get<DataLibrary>().characterPrefabs[1], transform); // Предположим, что у инженеров другой префаб (индекс 1)
        CharacterController crewController = newCrewMember.GetComponent<CharacterController>();
        if (crewController != null)
        {
            crewMembers.Add(crewController);
            allCrewMembers.Add(crewController);
        }
    }

    public override void HireNewCrewMember()
    {
        if (allCrewMembers.Count < blockData.MaxCrewUnlocked && allCrewMembers.Count < ServiceLocator.Get<StationController>().StationData.maxCrew.Value)
        {
            HireNewCrewMemberInternal();
            DistributeCrew(); // Используем метод из родительского класса
        }
        else
        {
            Debug.Log("Невозможно нанять нового члена экипажа в этом отделе.");
        }
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

        // Дополнительная инициализация списков, специфичная для инженерного блока
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