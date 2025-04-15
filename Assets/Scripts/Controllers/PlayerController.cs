using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private List<Resources> resources = new List<Resources>();
    
    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public async Task PlayerInitialization()
    {
        var loadData = await LoadPlayerData();
        playerData = loadData;
    }

    private async Task<PlayerData> LoadPlayerData()
    {
        // var testData = new PlayerData
        // {
        //     crewMood = 1000,
        //     playerCredits = 100,
        //     maxCrew = 1
        // };
        var loadData = await ServiceLocator.Get<CloudController>().LoadPlayerData();
        return loadData;
    }
}
