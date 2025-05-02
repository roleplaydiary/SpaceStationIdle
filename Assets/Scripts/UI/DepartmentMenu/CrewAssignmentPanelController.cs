using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class CrewAssignmentPanelController : MonoBehaviour
{
    [SerializeField] private Button addCrewToWork;
    [SerializeField] private Button removeCrewToWork;
    [SerializeField] private TMP_Text currentCrewAtWork;
    [SerializeField] private Button addCrewToRest;
    [SerializeField] private Button removeCrewToRest;
    [SerializeField] private TMP_Text currentCrewAtRest;
    [SerializeField] private TMP_Text currentCrewAtIdle;

    private StationBlockController blockController;
    private CompositeDisposable disposables;

    private void OnEnable()
    {
        Debug.Log("Crew Assignment Panel Enabled");
    }

    public void Initialize(StationBlockController block)
    {
        Debug.Log("Initializing crew assignment panel");
        blockController = block;

        disposables?.Dispose(); // Очищаем предыдущий, если он был
        disposables = new CompositeDisposable();
        
        // Подписываемся на клики кнопок
        addCrewToWork.OnClickAsObservable().Subscribe(_ => blockController?.AddCrewToWork()).AddTo(disposables);
        removeCrewToWork.OnClickAsObservable().Subscribe(_ =>blockController?.RemoveCrewFromWork()).AddTo(disposables);
        addCrewToRest.OnClickAsObservable().Subscribe(_ =>blockController?.AddCrewToRest()).AddTo(disposables);
        removeCrewToRest.OnClickAsObservable().Subscribe(_ =>blockController?.RemoveCrewFromRest()).AddTo(disposables);

        
        // Подписываемся на изменения количества персонала для обновления UI
        blockController?.GetCrewManager().workingCrew.ObserveCountChanged()
            .Subscribe( value => currentCrewAtWork.text = $"Currently working: {blockController.GetCrewManager().workingCrew.Count}").AddTo(disposables);
        blockController?.GetCrewManager().restingCrew.ObserveCountChanged()
            .Subscribe( value => currentCrewAtRest.text = $"Currently resting: {blockController.GetCrewManager().restingCrew.Count}").AddTo(disposables);
        blockController?.GetCrewManager().idleCrew.ObserveCountChanged()
            .Subscribe( value => currentCrewAtIdle.text = $"Currently idling: {blockController.GetCrewManager().idleCrew.Count}").AddTo(disposables);

        // Начальное обновление UI
        UpdateCrewCounts();
    }

    private void UpdateCrewCounts()
    {
        if (blockController != null)
        {
            currentCrewAtWork.text = $"Currently working: {blockController.GetCrewManager().workingCrew.Count}";
            currentCrewAtRest.text = $"Currently resting: {blockController.GetCrewManager().restingCrew.Count}";
            currentCrewAtIdle.text = $"Currently idling: {blockController.GetCrewManager().idleCrew.Count}";
        }
        else
        {
            currentCrewAtWork.text = "N/A";
            currentCrewAtRest.text = "N/A";
            currentCrewAtIdle.text = "N/A";
        }
    }

    private void OnDisable()
    {
        disposables.Dispose();
    }
}