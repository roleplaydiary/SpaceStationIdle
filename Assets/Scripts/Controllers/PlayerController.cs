using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private List<Resources> resources;
    
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
        if (loadData == null)
        {
            loadData = new PlayerData
            {
                crewMood = 1000,
                maxCrew = 1,
                playerCredits = 100
            };
        }
        playerData = loadData;
    }

    private async Task<PlayerData> LoadPlayerData()
    {
        var loadData = await ServiceLocator.Get<CloudController>().LoadPlayerData();
        return loadData;
    }
}
