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
        if (loadData == null || loadData.DepartmentData.Count == 0)
        {
            loadData = new StationData();
            loadData.Unlock(Department.Bridge);
            loadData.SetWorkbenchesLevelUnlocked(Department.Bridge, 1);
            loadData.SetMaxCrewUnlocked(Department.Bridge, 1);
            loadData.SetCurrentCrewHired(Department.Bridge, 1);
            loadData.MaxCrew.Value = 5;
            await ServiceLocator.Get<CloudController>().SaveStationData(stationData);
        }

        stationData = loadData; // Присваиваем загруженные данные stationData
        BlocksInitialize(StationData);
    }

    public void BlockCrewInitialize()
    {
        foreach (var block in stationBlocks)
        {
            var blockType = block.GetBlockType();
            if (stationData.IsUnlocked(blockType))
            {
                block.BlockCrewInitialization();
            }
        }
    }

    private void BlocksInitialize(StationData stationData)
    {
        this.stationData = stationData;
        foreach (var block in stationBlocks)
        {
            var blockType = block.GetBlockType();
            if (this.stationData.IsUnlocked(blockType))
            {
                block.BlockInitialization(this.stationData.DepartmentData[blockType]);
            }
            else
            {
                block.gameObject.SetActive(false);
            }
        }
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
                    block.BlockInitialization(stationData.DepartmentData[department]);
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
                block.AddWorkBench();
                break;
            }
        }
    }

    public void UpgradeDepartmentWorkbenchesMax(Department department)
    {
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == department)
            {
                block.UpgradeWorkBenchMax();
                break;
            }
        }
    }

    public void UpgradeStationMaxCrew()
    {
        stationData.MaxCrew.Value ++;
        Debug.Log($"Максимум экипажа на станции увеличено до {stationData.MaxCrew.Value}.");
    }

    public Transform GetRestPosition(CharacterController crewMember)
    {
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == Department.Bar || block.GetBlockType() == Department.Kitchen)
            {
                return block.GetBlockRestPosition(crewMember);
            }
        }
        return null;
    }

    public void ReleaseRestPosition(CharacterController crewMember)
    {
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == Department.Bar || block.GetBlockType() == Department.Kitchen)
            {
                block.ReleaseRestPosition(crewMember);
                return;
            }
        }
        
        Debug.LogError("ReleaseRestPosition: Позиция отдыха не была найдена");
    }

    public float GetStationCreditProductionValue()
    {
        float result = 0f;
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == Department.Bridge || block.GetBlockType() == Department.Cargo)
            {
                result += block.GetProductionValue();
            }
        }

        return result;
    }
    
    public float GetStationResearchProductionValue()
    {
        float result = 0f;
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == Department.Science)
            {
                result += block.GetProductionValue();
            }
        }

        return result;
    }
}