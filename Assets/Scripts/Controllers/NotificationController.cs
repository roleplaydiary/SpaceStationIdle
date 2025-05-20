using UnityEngine;
using System;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class NotificationController : MonoBehaviour
{
    // --- Android Specific Channel Settings ---
    // ID канала уведомлений для Android (обязательно для Android O/8.0+)
    private const string AndroidChannelId = "game_alerts_channel";
    private const string AndroidChannelName = "Game Alerts";
    private const string AndroidChannelDescription = "Important alerts and events from your game.";

    // --- iOS Specific Badge Settings (optional) ---
    // Если хотите управлять числом на иконке приложения на iOS
    private int currentIosBadgeCount = 0; 

    void Awake()
    {
        ServiceLocator.Register(this); 
        
        Debug.Log("NotificationController: Initializing notification system.");

#if UNITY_ANDROID
        // Инициализация канала уведомлений для Android. 
        // Важно: Вызывать только один раз при старте приложения.
        InitializeAndroidNotificationsChannel();
#endif

#if UNITY_IOS
        // Запрос разрешения на уведомления для iOS.
        RequestIOSAuthorization();
#endif

        // Очищаем все УЖЕ ДОСТАВЛЕННЫЕ уведомления при каждом старте игры.
        // Это предотвращает появление старых уведомлений, если игра была закрыта, а затем открыта вручную.
        ClearAllDeliveredNotifications();

        // Проверяем, было ли приложение запущено нажатием на уведомление.
        // Если да, обрабатываем данные и очищаем уведомление.
        CheckForNotificationLaunch();
    }

    /// <summary>
    /// Инициализирует канал уведомлений для Android.
    /// </summary>
    private void InitializeAndroidNotificationsChannel()
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel(
            AndroidChannelId,
            AndroidChannelName,
            AndroidChannelDescription,
            Importance.High // Можно поставить High для более заметных уведомлений
        );
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        Debug.Log($"Android Notification Channel '{AndroidChannelName}' registered with ID: {AndroidChannelId}.");
#endif
    }

    /// <summary>
    /// Запрашивает разрешение на отправку уведомлений на iOS.
    /// </summary>
    private void RequestIOSAuthorization()
    {
#if UNITY_IOS
        iOSNotificationCenter.RequestAuthorization((AuthorizationStatus status) =>
        {
            if (status == AuthorizationStatus.Authorized)
            {
                Debug.Log("iOS: Notification authorization granted.");
                // Fetch current badge count if you are managing it
                currentIosBadgeCount = iOSNotificationCenter.ApplicationBadge;
            }
            else
            {
                Debug.LogWarning("iOS: Notification authorization denied.");
            }
        });
#endif
    }

    /// <summary>
    /// Планирует локальное пуш-уведомление, которое будет отображено, когда игрок вне игры.
    /// </summary>
    /// <param name="title">Заголовок уведомления.</param>
    /// <param name="body">Текст уведомления.</param>
    /// <param name="delay">Задержка до отправки уведомления (от текущего момента).</param>
    /// <param name="notificationData">Опциональные данные в виде строки (например, ID ивента, JSON).</param>
    public void ScheduleNotification(string title, string body, TimeSpan delay, string notificationData = "")
    {
        DateTime fireTime = DateTime.Now.Add(delay);
        Debug.Log($"Scheduling notification: '{title}' - '{body}' to fire at {fireTime} (in {delay.TotalMinutes:F1} minutes). Data: '{notificationData}'");

#if UNITY_ANDROID
        var notification = new AndroidNotification
        {
            Title = title,
            Text = body,
            FireTime = fireTime,
            SmallIcon = "icon_0", // Маленькая иконка (требуется в Android Player Settings)
            LargeIcon = "icon_1", // Большая иконка (требуется в Android Player Settings)
            IntentData = notificationData // Дополнительные данные, доступны при запуске через уведомление
            //ChannelId = AndroidChannelId
        };
        AndroidNotificationCenter.SendNotification(notification, AndroidChannelId);
        Debug.Log($"Android notification '{title}' scheduled.");
#elif UNITY_IOS
        // Увеличиваем счетчик на иконке приложения
        currentIosBadgeCount++;
        iOSNotificationCenter.ApplicationBadge = currentIosBadgeCount;

        var notification = new iOSNotification
        {
            Title = title,
            Body = body,
            ShowInForeground = false, // Уведомление показывается, только когда игра НЕ активна
            // LaunchActivity = "unity.MainActivity", // Обычно не требуется указывать явно, Unity делает это по умолчанию
            Badge = currentIosBadgeCount, // Отображаем количество на иконке приложения
            Data = notificationData, // Дополнительные данные
            Trigger = new iOSNotificationTimeIntervalTrigger
            {
                TimeInterval = delay,
                Repeats = false // Уведомление сработает только один раз
            }
        };
        iOSNotificationCenter.ScheduleNotification(notification);
        Debug.Log($"iOS notification '{title}' scheduled.");
#else
        Debug.LogWarning($"Notifications are not supported on this platform. Scheduled: {title} - {body}");
#endif
    }

    /// <summary>
    /// Отменяет все запланированные, но еще не отправленные уведомления.
    /// Обычно вызывается при выходе из игры, чтобы перепланировать уведомления.
    /// </summary>
    public void CancelAllScheduledNotifications()
    {
        Debug.Log("Cancelling all scheduled notifications.");
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        Debug.Log("Android: All scheduled notifications cancelled.");
#endif
#if UNITY_IOS
        iOSNotificationCenter.RemoveAllPendingNotificationRequests();
        // Сбрасываем счетчик на иконке, так как все уведомления отменены
        iOSNotificationCenter.ApplicationBadge = 0; 
        currentIosBadgeCount = 0;
        Debug.Log("iOS: All pending notification requests removed.");
#endif
    }

    /// <summary>
    /// Очищает все уведомления, которые уже были показаны в центре уведомлений.
    /// Вызывается при запуске игры.
    /// </summary>
    private void ClearAllDeliveredNotifications()
    {
        Debug.Log("Clearing all delivered notifications.");
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        Debug.Log("Android: All currently displayed notifications cleared.");
#endif
#if UNITY_IOS
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
        // Сбрасываем счетчик на иконке при очистке доставленных уведомлений
        iOSNotificationCenter.ApplicationBadge = 0;
        currentIosBadgeCount = 0;
        Debug.Log("iOS: All delivered notifications removed.");
#endif
    }
    
    /// <summary>
    /// Проверяет, было ли приложение запущено через уведомление, и обрабатывает его данные.
    /// </summary>
    public void CheckForNotificationLaunch()
    {
        Debug.Log("Checking for notification launch data.");
#if UNITY_ANDROID
        // Android-специфическая логика: получение данных о запуске через уведомление.
        // GetLastNotificationIntent() возвращает данные уведомления, если приложение было запущено с него.
        var notificationIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notificationIntentData != null)
        {
            Debug.Log("Android: Application launched from notification via GetLastNotificationIntent.");
            // Обработка данных уведомления, если оно было запущено через уведомление
            HandleNotificationData(notificationIntentData.Notification.IntentData);
            // Опционально: отменяем это конкретное уведомление из шторки
            AndroidNotificationCenter.CancelNotification(notificationIntentData.Id);
        }
        else
        {
            Debug.Log("Android: No notification launch data found or app launched normally.");
        }
#endif
#if UNITY_IOS
        iOSNotificationCenter.GetLastRespondedNotification((notification) =>
        {
            if (notification != null)
            {
                Debug.Log($"iOS: Application launched from notification. Title: {notification.Title}, Body: {notification.Body}, Data: {notification.Data}");
                HandleNotificationData(notification.Data);
                iOSNotificationCenter.RemoveDeliveredNotification(notification.Identifier);
            }
        });
#endif
    }

#if UNITY_ANDROID
    /// <summary>
    /// Обработчик получения уведомления для Android (когда игра уже запущена).
    /// </summary>
    private void OnAndroidNotificationReceived(AndroidNotificationIntentData data)
    {
        Debug.Log($"Android: Notification received while app was running. Title: {data.Notification.Title}, Body: {data.Notification.Text}, Data: {data.Notification.IntentData}");
        // Здесь вы можете обработать 'data.Notification.IntentData'
        HandleNotificationData(data.Notification.IntentData);
        // Если вы хотите убрать уведомление из шторки после обработки, используйте:
        // AndroidNotificationCenter.CancelNotification(data.Id);
    }
#endif

    /// <summary>
    /// Универсальный метод для обработки данных уведомления.
    /// Здесь вы можете парсить JSON или выполнять другие действия.
    /// </summary>
    /// <param name="data">Строка данных из уведомления.</param>
    private void HandleNotificationData(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            Debug.Log("Notification data is empty.");
            return;
        }

        Debug.Log($"Handling notification data: {data}");
        // Пример обработки:
        // if (data == "station_shutdown")
        // {
        //     Debug.Log("Player was notified about station shutdown. Show specific UI or perform action.");
        //     // Например, показать внутриигровой попап или перейти на определенный экран
        // }
        // else if (data.Contains("EventId")) // Предполагаем JSON-данные для ивентов
        // {
        //     try
        //     {
        //         // Здесь вы можете десериализовать JSON, если передаете его в notificationData
        //         // var eventInfo = JsonUtility.FromJson<YourEventDataClass>(data);
        //         // Debug.Log($"Event notification received: {eventInfo.EventId}");
        //         // TriggerGameEvent(eventInfo.EventId);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError($"Failed to parse event data JSON: {e.Message}");
        //     }
        // }
        // Добавьте здесь свою игровую логику для обработки данных
    }
}