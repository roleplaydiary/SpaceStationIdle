using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private StationController stationController;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private DataLibrary dataLibrary;

    private void Start()
    {
        ServiceLocator.Register(dataLibrary);
        GameInitialization();
    }

    private void GameInitialization()
    {
        playerController.PlayerInitialization(); // TODO: await
        stationController.StationInitializate(); // TODO: await
    }
}
