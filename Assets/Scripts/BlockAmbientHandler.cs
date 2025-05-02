using Controllers;
using UniRx;
using UnityEngine;

public class BlockAmbientHandler : MonoBehaviour
{
    [SerializeField] private string audioClipName;
    [SerializeField] private AudioSource ambientSoundSource;
    [SerializeField] private Department department;

    private CompositeDisposable disposables = new CompositeDisposable();

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        var gameController = ServiceLocator.Get<GameController>();
        if (gameController == null)
        {
            Debug.LogError("BlockAmbientHandler: GameController is null");
            return;
        }

        gameController.OnGameInitialized
            .Where(initialized => initialized)
            .Subscribe(_ => OnGameInitializedHandler())
            .AddTo(disposables);
    }

    private void OnGameInitializedHandler()
    {
        var stationController = ServiceLocator.Get<StationController>();
        if (stationController != null && stationController.StationData != null && stationController.StationData.IsUnlocked(department))
        {
            var dataLibrary = ServiceLocator.Get<DataLibrary>();
            var audioManager = ServiceLocator.Get<AudioManager>();

            if (dataLibrary != null && audioManager != null && ambientSoundSource != null)
            {
                ambientSoundSource.clip = dataLibrary.soundLibrary.GetDepartmentAmbientSound(audioClipName);
                if (ambientSoundSource.clip != null && !ambientSoundSource.isPlaying)
                {
                    ambientSoundSource.Play();
                }
                audioManager.ambientVolume
                    .Subscribe(value => ambientSoundSource.volume = value)
                    .AddTo(disposables);
            }
            else
            {
                Debug.LogError("BlockAmbientHandler: DataLibrary, AudioManager, or ambientSoundSource is null during ambient sound initialization for " + department);
            }
        }
        else
        {
            Debug.Log("BlockAmbientHandler: Department " + department + " is locked at game start.");
            // TODO: Подписаться на событие разблокировки отдела для последующей инициализации
            if (stationController != null)
            {
                // stationController.OnDepartmentUnlocked
                //     .Where(unlockedDepartment => unlockedDepartment == department)
                //     .Subscribe(_ => ReinitializeAmbientSound())
                //     .AddTo(disposables);
            }
        }
    }

    private void ReinitializeAmbientSound()
    {
        Debug.Log("BlockAmbientHandler: Re-initializing ambient sound for unlocked department " + department);
        var dataLibrary = ServiceLocator.Get<DataLibrary>();
        var audioManager = ServiceLocator.Get<AudioManager>();

        if (dataLibrary != null && audioManager != null && ambientSoundSource != null)
        {
            ambientSoundSource.clip = dataLibrary.soundLibrary.GetDepartmentAmbientSound(audioClipName);
            if (ambientSoundSource.clip != null && !ambientSoundSource.isPlaying)
            {
                ambientSoundSource.Play();
            }
            // Подписываемся на громкость только один раз при первой инициализации или переинициализации
            audioManager.ambientVolume
                .Subscribe(value => ambientSoundSource.volume = value)
                .AddTo(disposables);
        }
        else
        {
            Debug.LogError("BlockAmbientHandler: DataLibrary, AudioManager, or ambientSoundSource is null during re-initialization for " + department);
        }
    }

    private void OnDestroy()
    {
        disposables.Clear();
    }
}