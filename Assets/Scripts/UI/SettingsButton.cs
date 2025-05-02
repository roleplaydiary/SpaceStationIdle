using Controllers;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    private void Start()
    {
        var gameController = ServiceLocator.Get<GameController>();
        if (gameController == null)
        {
            Debug.LogError("BlockAmbientHandler: GameController is null");
            return;
        }

        gameController.OnGameInitialized.Where(initialized => initialized).Subscribe(a =>
        {
            var uiController = ServiceLocator.Get<UIController>();
            var button = gameObject.GetComponent<Button>();
            button.OnClickAsObservable().Subscribe(_ =>
            {
                uiController.SettingsMenuShow();
            }).AddTo(this);
        }).AddTo(this);
    }
}
