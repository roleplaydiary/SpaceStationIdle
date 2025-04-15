using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private StationController stationController;
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
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
