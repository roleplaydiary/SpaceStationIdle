using System;
using System.Collections.Generic;
using UniRx;
using Unity.Services.CloudSave;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusicSource;
    public ReactiveProperty<float> backgroundMusicVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость музыки
    public AudioSource uiSoundSource;
    public ReactiveProperty<float> effectsVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость эффектов (включая отсеки)
    public ReactiveProperty<float> ambientVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость эмбиента
    public static float sound_volume_constant = 0.1f;

    private static string BackgroundMusicVolumeKey = "backgroundMusicVolume";
    private static string EffectsVolumeKey = "effectsVolume";
    private static string AmbientVolumeKey = "ambientVolume";

    private async void Awake()
    {
        ServiceLocator.Register(this);

        backgroundMusicVolume.Subscribe(volume =>
        {
            if (backgroundMusicSource != null)
            {
                backgroundMusicSource.volume = Mathf.Clamp01(volume);
            }
        }).AddTo(this);

        effectsVolume.Subscribe(volume =>
        {
            if (uiSoundSource != null)
            {
                uiSoundSource.volume = Mathf.Clamp01(volume);
            }
        }).AddTo(this);
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (backgroundMusicSource != null && clip != null)
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.Play();
        }
    }

    public float UpdateBackgroundMusicVolume(float volumeChange)
    {
        float newVolume = backgroundMusicVolume.Value + volumeChange;
        backgroundMusicVolume.Value = Mathf.Clamp01(newVolume);
        SaveSoundSettings(); // Сохраняем настройки при изменении
        return newVolume;
    }

    public void PlayUISound(AudioClip clip)
    {
        if (uiSoundSource != null && clip != null)
        {
            uiSoundSource.PlayOneShot(clip);
        }
    }

    // Метод для установки громкости эффектов (будет вызываться из настроек)
    public float UpdateEffectsVolume(float volume)
    {
        float newVolume = effectsVolume.Value + volume;
        effectsVolume.Value = Mathf.Clamp01(newVolume);
        SaveSoundSettings(); // Сохраняем настройки при изменении
        return newVolume;
    }

    public float UpdateAmbientVolume(float volume)
    {
        float newVolume = ambientVolume.Value + volume;
        ambientVolume.Value = Mathf.Clamp01(newVolume);
        SaveSoundSettings(); // Сохраняем настройки при изменении
        return newVolume;
    }

    // Метод для сохранения настроек звука в Cloud Save
    public async void SaveSoundSettings()
    {
        var cloudController = ServiceLocator.Get<CloudController>();
        if (cloudController != null)
        {
            Dictionary<string, object> soundSettings = new Dictionary<string, object>();
            soundSettings[BackgroundMusicVolumeKey] = backgroundMusicVolume.Value;
            soundSettings[EffectsVolumeKey] = effectsVolume.Value;
            soundSettings[AmbientVolumeKey] = ambientVolume.Value;

            try
            {
                await cloudController.SaveGenericData("sound_settings", soundSettings);
                Debug.Log("Настройки звука сохранены в Cloud Save.");
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"Ошибка сохранения настроек звука: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("CloudController не найден для сохранения настроек звука.");
        }
    }

    // Метод для загрузки настроек звука из Cloud Save
    public async System.Threading.Tasks.Task LoadSoundSettings()
    {
        var cloudController = ServiceLocator.Get<CloudController>();
        if (cloudController != null)
        {
            try
            {
                var loadedData = await cloudController.LoadGenericData("sound_settings");
                if (loadedData != null)
                {
                    if (loadedData.TryGetValue(BackgroundMusicVolumeKey, out var bgVolume))
                    {
                        backgroundMusicVolume.Value = Convert.ToSingle(bgVolume);
                    }
                    if (loadedData.TryGetValue(EffectsVolumeKey, out var fxVolume))
                    {
                        effectsVolume.Value = Convert.ToSingle(fxVolume);
                    }
                    if (loadedData.TryGetValue(AmbientVolumeKey, out var ambVolume))
                    {
                        ambientVolume.Value = Convert.ToSingle(ambVolume);
                    }
                    Debug.Log("Настройки звука загружены из Cloud Save.");
                }
                else
                {
                    Debug.Log("Нет сохраненных настроек звука в Cloud Save, используются настройки по умолчанию.");
                }
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"Ошибка загрузки настроек звука: {e.Message}");
            }
        }
        else
        {
            Debug.LogError("CloudController не найден для загрузки настроек звука.");
        }
    }
}