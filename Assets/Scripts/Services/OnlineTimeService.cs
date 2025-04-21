using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;

public static class OnlineTimeService
{
    private const string TimeServerURL = "https://worldtimeapi.org/api/utc/now";

    public static Task<DateTime?> GetUTCTimeAsync()
    {
        var tcs = new TaskCompletionSource<DateTime?>();

        using (UnityWebRequest request = UnityWebRequest.Get(TimeServerURL))
        {
            var asyncOperation = request.SendWebRequest();

            asyncOperation.completed += (operation) =>
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Ошибка при получении онлайн-времени: {request.error}");
                    tcs.SetResult(null);
                    return;
                }

                try
                {
                    string jsonResponse = request.downloadHandler.text;
                    var timeData = JsonUtility.FromJson<TimeResponse>(jsonResponse);
                    tcs.SetResult(DateTime.Parse(timeData.datetime).ToUniversalTime());
                }
                catch (Exception e)
                {
                    Debug.LogError($"Ошибка при парсинге времени: {e.Message}");
                    tcs.SetResult(null);
                }
            };
        }

        return tcs.Task; // Возвращаем Task из TaskCompletionSource
    }

    [Serializable]
    private struct TimeResponse
    {
        public string datetime;
    }
}