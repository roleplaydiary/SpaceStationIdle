using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

public class DebugUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text creditsText;
    [SerializeField] private TMP_Text currentCrewText;
    [SerializeField] private TMP_Text maxCrewText;
    [SerializeField] private TMP_Text bridgeCrewAtWorkText;
    [SerializeField] private TMP_Text bridgeCrewAtRestText;
    [SerializeField] private TMP_Text bridgeCrewAtIdleText;

    private PlayerController playerController;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void DebugUIInitialize()
    {
        playerController = ServiceLocator.Get<PlayerController>();
        if (playerController != null && creditsText != null)
        {
            playerController.GetPlayerData().playerCredits.Subscribe(credits =>
            {
                creditsText.text = $"Credits: {credits}";
            }).AddTo(this);
        }

        // Подписка на другие ReactiveProperty (примеры)
        if (playerController != null && maxCrewText != null)
        {
            playerController.GetPlayerData().maxCrew.Subscribe(max =>
            {
                maxCrewText.text = $"Max Crew: {max}";
            }).AddTo(this);
        }

        // Вам нужно будет получить доступ к данным о экипаже мостика из StationController или другого места,
        // где эта информация хранится и сделать аналогичную подписку, если эти данные реактивны.
        // Пример (предполагая, что в StationController есть ReactiveProperty для экипажа мостика):
        var stationController = ServiceLocator.Get<StationController>();
        if (stationController != null)
        {
            // Пример подписки (предполагается наличие реактивных свойств в StationController)
            // stationController.BridgeCrewAtWork.Subscribe(count => bridgeCrewAtWorkText.text = $"Bridge Work: {count}").AddTo(this);
            // stationController.BridgeCrewAtRest.Subscribe(count => bridgeCrewAtRestText.text = $"Bridge Rest: {count}").AddTo(this);
            // stationController.BridgeCrewAtIdle.Subscribe(count => bridgeCrewAtIdleText.text = $"Bridge Idle: {count}").AddTo(this);
        }
        
        Debug.Log($"Инициализация DebugUIController завершена");
    }

    // Метод OnDestroy для очистки подписок (хотя AddTo(this) должен это делать)
    private void OnDestroy()
    {
        // Все подписки, добавленные через .AddTo(this), будут автоматически отписаны.
    }
}