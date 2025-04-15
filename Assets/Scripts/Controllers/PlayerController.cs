using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerCredits = 100;
    [SerializeField] private int maxCrew = 10;
    [SerializeField] private float crewMood = 1000;
    [SerializeField] private List<Resources> resources = new List<Resources>();

    public void PlayerInitialization()
    {
        var loadData = LoadPlayerData();
        playerCredits = loadData.playerCredits;
        maxCrew = loadData.maxCrew;
        crewMood = loadData.crewMood;
    }

    private PlayerData LoadPlayerData()
    {
        var testData = new PlayerData
        {
            crewMood = 1000,
            playerCredits = 100,
            maxCrew = 1
        };
        return testData;
    }
}
