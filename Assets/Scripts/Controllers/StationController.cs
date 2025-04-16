using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<StationBlockController> stationBlocks;
    [SerializeField] private StationData stationData;
    public StationData StationData { get => stationData;}
    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public async Task StationInitializate()
    {
        var loadData = await ServiceLocator.Get<CloudController>().LoadStationData();
        if (loadData == null || !loadData.departmentData.ContainsKey(Department.Bridge))
        {
            loadData = new StationData();
            loadData.Unlock(Department.Bridge);
            loadData.SetWorkbenchesLevelUnlocked(Department.Bridge, 1);
            loadData.SetMaxCrewUnlocked(Department.Bridge, 1);
            loadData.SetCurrentCrewHired(Department.Bridge, 1);
            loadData.maxCrew.Value = 5;
            loadData.crewMood.Value = 1000f;
        }
        BlocksInitialize(loadData);
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

        // Нанимаем сотрудника
        stationData.departmentData[department].CurrentCrewHired++;

        // Находим соответствующий блок и просим его создать нового члена экипажа
        foreach (var block in stationBlocks)
        {
            if (block.GetBlockType() == department)
            {
                block.HireNewCrewMember();
                break; // Предполагаем один контроллер на отдел
            }
        }

        // Сохраняем обновленные данные
        await ServiceLocator.Get<CloudController>().SaveStationData(stationData);
        Debug.Log($"В отдел {department} нанят новый сотрудник. Текущее количество: {stationData.departmentData[department].CurrentCrewHired}");
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

            // Find the corresponding StationBlockController and activate it
            foreach (var block in stationBlocks)
            {
                if (block.GetBlockType() == department)
                {
                    block.BlockInitialization(stationData.departmentData[department]);
                    block.gameObject.SetActive(true);
                    break; // Assuming only one block per department
                }
            }

            // Save the updated station data
            await ServiceLocator.Get<CloudController>().SaveStationData(stationData);
            Debug.Log($"Отдел {department} разблокирован и данные сохранены.");
        }
        else
        {
            Debug.Log($"Отдел {department} уже разблокирован.");
        }
    }

    public StationData GetStationData()
    {
        return stationData;
    }
}
