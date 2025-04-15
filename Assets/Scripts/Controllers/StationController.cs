using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private List<StationBlockController> stationBlocks = new List<StationBlockController>();

    public void BlocksInitialize()
    {
        foreach (var block in stationBlocks)
        {
            block.BlockInitialization();
            // Инициализация каждого блока
            // Количество экипажа в блоке
            // Параметры блока(Апгрейды)
        }
    }
}
