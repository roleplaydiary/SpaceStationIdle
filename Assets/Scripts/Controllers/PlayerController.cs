using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int playerCredits = 100;
    [SerializeField] private int maxCrew = 10;
    [SerializeField] private float crewMood = 1000;
    //resources - Пусть будет массив ресурсов с айди, названием, количеством и максимумом

    private void PlayerInitialization()
    {
        // Загрузка данных игрока
    }
}
