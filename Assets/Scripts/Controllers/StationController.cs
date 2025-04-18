using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<StationBlockController> stationBlocks;
    public List<StationBlockController> StationBlocks { get => stationBlocks; }
    [SerializeField] private StationData stationData;
    public StationData StationData { get => stationData; }

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public async Task StationInitializate()
    {
        var loadData = await ServiceLocator.Get<CloudController>().LoadStationData();
        if (loadData == null)
        {
            loadData = new StationData();
            loadData.Unlock(Department.Bridge);
            loadData.SetWorkbenchesLevelUnlocked(Department.Bridge, 1);
            loadData.SetMaxCrewUnlocked(Department.Bridge, 1);
            loadData.SetCurrentCrewHired(Department.Bridge, 1);
            loadData.maxCrew.Value = 5;
            loadData.crewMood.Value = 1000f;
        }

        stationData = loadData; // Присваиваем загруженные данные stationData
        BlocksInitialize(StationData);
        SubscribeToEnergyChanges(); // Подписываемся на изменения энергии в блоках
    }

    private void SubscribeToEnergyChanges()
    {
        foreach (var block in stationBlocks)
        {
            if (block.EnergyController != null)
            {
                block.EnergyController.currentEnergyProduction.Subscribe(_ => UpdateTotalStationEnergy()).AddTo(this);
                block.EnergyController.currentEnergyConsumption.Subscribe(_ => UpdateTotalStationEnergy()).AddTo(this);
            }
        }
        UpdateTotalStationEnergy(); // Начальный расчет
    }

    private void BlocksInitialize(StationData stationData)
    {
        this.stationData = stationData;
        foreach (var block in stationBlocks)
        {
            var blockType = block.GetBlockType();
            if (this.stationData.IsUnlocked(blockType))
            {
                block.BlockInitialization(this.stationData.departmentData[blockType]);
            }
            else
            {
                block.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateTotalStationEnergy()
    {
        float totalProduction = 0f;
        float totalConsumption = 0f;

        foreach (var block in stationBlocks)
        {
            if (block.EnergyController)
            {
                totalProduction += block.EnergyController.currentEnergyProduction.Value;
                totalConsumption += block.EnergyController.currentEnergyConsumption.Value;
            }
        }

        StationData.stationEnergy.Value = totalProduction - totalConsumption;
    }

    public void TestHireCrewMember()
    {
        HireCrewMember(Department.Engineer);
    }

    public async void HireCrewMember(Department department)
    {
        if (!stationData.IsUnlocked(department))
        {
            Debug.Log($"Отдел {department} не разблокирован. Невозможно нанять сотрудника.");
            return;
        }

        if (stationData.departmentData[department].CurrentCrewHired >=
            stationData.departmentData[department].MaxCrewUnlocked)
        {
            Debug.Log($"Достигнут лимит личного состава в отделе {department}.");
            return;
        }

        if (stationData.departmentData.Values.Sum(data => data.CurrentCrewHired) >= stationData.maxCrew.Value)
        {
            Debug.Log("Достигнут лимит личного состава станции.");
            return;
        }

        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == department)
            {
                block.HireNewCrewMember();
                break;
            }
        }

        await ServiceLocator.Get<CloudController>().SaveStationData(stationData);
        Debug.Log($"Запрос на найм сотрудника в отдел {department} выполнен.");
        Debug.Log($"Текущее количество сотрудников в отделе {department}: {stationData.departmentData[department].CurrentCrewHired}");
    }

    public void TestUnlockEngineering()
    {
        UnlockStationBlock(Department.Engineer);
    }

    public async void UnlockStationBlock(Department department)
    {
        if (!stationData.IsUnlocked(department))
        {
            stationData.Unlock(department);

            foreach (var block in stationBlocks)
            {
                if (block.GetBlockType() == department)
                {
                    block.BlockInitialization(stationData.departmentData[department]);
                    block.gameObject.SetActive(true);
                    break;
                }
            }

            await ServiceLocator.Get<CloudController>().SaveStationData(stationData);
            Debug.Log($"Отдел {department} разблокирован и данные сохранены.");
        }
        else
        {
            Debug.Log($"Отдел {department} уже разблокирован.");
        }
    }

    public void UpgradeDepartmentMaxCrew(Department department)
    {
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == department)
            {
                block.UpgradeMaxCrew();
                break;
            }
        }
    }

    public void UpgradeDepartmentWorkbenches(Department department)
    {
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == department)
            {
                block.UnlockWorkBench();
                break;
            }
        }
    }

    public StationData GetStationData()
    {
        return stationData;
    }
}