using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

public static class OnlineTimeService
{
    private const string TimeServerURL = "https://worldtimeapi.org/api/timezone/utc";

    public static async Task<DateTime?> GetUTCTimeAsync()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(TimeServerURL))
        {
            request.timeout = 10; // Добавим таймаут, чтобы избежать зависаний

            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Ошибка при получении онлайн-времени: {request.error}");
                return null;
            }

            try
            {
                string jsonResponse = request.downloadHandler.text;
                var timeData = JsonUtility.FromJson<TimeResponse>(jsonResponse);
                return DateTime.Parse(timeData.utc_datetime).ToUniversalTime();
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка при парсинге времени: {e.Message} - JSON: {request.downloadHandler.text}");
                return null;
            }
        }
    }

    [Serializable]
    private struct TimeResponse
    {
        public string utc_datetime; // Используем это поле, так как оно содержит UTC время
    }
}