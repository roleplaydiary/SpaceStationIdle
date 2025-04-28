using UniRx;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusicSource;
    public ReactiveProperty<float> backgroundMusicVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость музыки
    public AudioSource uiSoundSource;
    public ReactiveProperty<float> effectsVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость эффектов (включая отсеки)
    public ReactiveProperty<float> ambientVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость эффектов (включая отсеки)
    public static float sound_volume_constant = 0.1f;

    private void Awake()
    { 
        ServiceLocator.Register(this);

        // TODO: add sound volume loading
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

    public void PauseBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Pause();
        }
    }

    public float UpdateBackgroundMusicVolume(float volumeChange)
    {
        float newVolume = backgroundMusicVolume.Value + volumeChange;
        backgroundMusicVolume.Value = Mathf.Clamp01(newVolume);
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
        return newVolume;
    }
    
    public float UpdateAmbientVolume(float volume)
    {
        float newVolume = ambientVolume.Value + volume;
        ambientVolume.Value = Mathf.Clamp01(newVolume);
        return newVolume;
    }
}