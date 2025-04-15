using UnityEngine;
using UnityEngine.UI; // Необходимо для работы с UI элементами

public class TestButton : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Компонент Button не найден на GameObject!");
        }
    }

    async void OnButtonClick()
    {
        var playerData = ServiceLocator.Get<PlayerController>().GetPlayerData();
        await ServiceLocator.Get<CloudController>().SavePlayerData(playerData);
    }
}