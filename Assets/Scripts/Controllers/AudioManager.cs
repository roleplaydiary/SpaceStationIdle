using UniRx;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusicSource;
    public ReactiveProperty<float> backgroundMusicVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость музыки
    public AudioSource uiSoundSource;
    public ReactiveProperty<float> effectsVolume { get; private set; } = new ReactiveProperty<float>(0.1f); // Начальная громкость эффектов (включая отсеки)

    private void Awake()
    { 
        ServiceLocator.Register(this);

        // Подписываемся на изменения громкости музыки и применяем их
        backgroundMusicVolume.Subscribe(volume =>
        {
            if (backgroundMusicSource != null)
            {
                backgroundMusicSource.volume = Mathf.Clamp01(volume);
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

    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume.Value = Mathf.Clamp01(volume);
    }

    public void PlayUISound(AudioClip clip)
    {
        if (uiSoundSource != null && clip != null)
        {
            uiSoundSource.PlayOneShot(clip);
        }
    }

    // Метод для установки громкости эффектов (будет вызываться из настроек)
    public void SetEffectsVolume(float volume)
    {
        effectsVolume.Value = Mathf.Clamp01(volume);
    }
}