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
        stationController.StationInitializate();
        // Последовательная инициализация других контроллеров:
        // Загрузка прогресса игрока
        // Загрузка прогресса станции
    }
}
