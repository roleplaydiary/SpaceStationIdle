using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<GameObject> stationBlocks = new List<GameObject>();

    private void BlocksInitialize()
    {
        foreach (var block in stationBlocks)
        {
            //block.Initialize();
            // Инициализация каждого блока
            // Количество экипажа в блоке
            // Параметры блока(Апгрейды)
        }
    }
}
