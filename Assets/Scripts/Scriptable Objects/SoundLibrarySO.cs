using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

[System.Serializable]
public struct NamedAudioClip
{
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibrarySO : ScriptableObject
{
    [Header("Фоновая музыка")]
    public List<AudioClip> backgroundMusicTracks;

    [Header("UI звуки")]
    public List<NamedAudioClip> uiSoundsList;

    [FormerlySerializedAs("departmentSoundsList")] [Header("Звуки отсеков")]
    public List<NamedAudioClip> ambientSoundsList;

    private Dictionary<string, AudioClip> uiSoundsCache;
    private Dictionary<string, AudioClip> ambientSoundsCache;

    private void OnEnable()
    {
        // Кэшируем списки в словари при загрузке ассета
        uiSoundsCache = uiSoundsList.ToDictionary(nac => nac.name, nac => nac.clip);
        ambientSoundsCache = ambientSoundsList.ToDictionary(nac => nac.name, nac => nac.clip);
    }

    // Метод для получения UI звука по имени
    public AudioClip GetUISound(string soundName)
    {
        if (uiSoundsCache != null && uiSoundsCache.TryGetValue(soundName, value: out var sound))
        {
            return sound;
        }
        else
        {
            Debug.LogWarning($"UI звук с именем '{soundName}' не найден в библиотеке.");
            return null;
        }
    }

    // Метод для получения звука отсека по имени
    public AudioClip GetDepartmentSound(string soundName)
    {
        if (ambientSoundsCache != null && ambientSoundsCache.TryGetValue(soundName, out var sound))
        {
            return sound;
        }
        else
        {
            Debug.LogWarning($"Звук отсека с именем '{soundName}' не найден в библиотеке.");
            return null;
        }
    }

    // Метод для случайного получения трека фоновой музыки (остается без изменений)
    //public AudioClip GetRandomBackgroundMusic() { /* ... */ }
}