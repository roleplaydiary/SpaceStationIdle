using System;
using TMPro;
using UI;
using UniRx;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeMessageController : MonoBehaviour
{
    // Структура для параметров Remote Config (обязательно)
    public struct UserAttributes { }
    public struct AppAttributes { }

    [Header("UI Элементы")]
    [SerializeField] private GameObject welcomePanelContent;
    [SerializeField] private TMP_Text welcomeTitleText;
    [SerializeField] private TMP_Text welcomeBodyText;
    [SerializeField] private Button closeButton;
    
    private WelcomeData welcomeData;

    private void Awake()
    {
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().WelcomeMessageHide();
        }).AddTo(this);
    }

    public void Initialize()
    {
        if (welcomePanelContent != null)
        {
            welcomePanelContent.SetActive(false);
        }

        // Подписываемся на событие завершения получения настроек Remote Config
        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetchCompleted;
        // Запускаем процесс получения конфигурации
        RemoteConfigService.Instance.FetchConfigs(new UserAttributes(), new AppAttributes());
    }

    void OnDestroy()
    {
        // Отписываемся от события, чтобы избежать утечек памяти
        RemoteConfigService.Instance.FetchCompleted -= OnRemoteConfigFetchCompleted;
    }

    public void Hide()
    {
        welcomePanelContent.SetActive(false);
    }

    private void OnRemoteConfigFetchCompleted(ConfigResponse response)
    {
        Debug.Log($"Remote Config: FetchCompleted status: {response.status}");

        // Попытка получить JSON-строку из Remote Config
        string jsonMessage = RemoteConfigService.Instance.appConfig.GetJson("welcome_message", "");

        if (!string.IsNullOrEmpty(jsonMessage))
        {
            try
            {
                // Десериализация JSON-строки в объект WelcomeData
                welcomeData = JsonUtility.FromJson<WelcomeData>(jsonMessage);
                Debug.Log($"Remote Config: Successfully parsed WelcomeData. Title: {welcomeData.Title}");
                ShowWelcomeScreen(welcomeData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Remote Config: Failed to parse WelcomeDataJson: {e.Message}. Using default data.");
                welcomeData = WelcomeData.GetDefault(); // Используем данные по умолчанию в случае ошибки парсинга
            }
        }
        else
        {
            Debug.LogWarning("Remote Config: WelcomeDataJson is empty or not found. Using default data.");
        }
    }

    private void ShowWelcomeScreen(WelcomeData data)
    {
        if (welcomePanelContent != null && welcomeTitleText != null && welcomeBodyText != null)
        {
            welcomeTitleText.text = data.Title;
            welcomeBodyText.text = data.Text;
            welcomePanelContent.SetActive(true);
        }
        else
        {
            Debug.LogError("WelcomeScreenManager: UI elements (Panel, Title Text, Body Text) are not assigned!");
        }
    }
}
