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
    private CompositeDisposable disposables = new CompositeDisposable();

    public void Initialize(StationBlockController block)
    {
        blockController = block;

        // Подписываемся на клики кнопок
        addCrewToWork.OnClickAsObservable().Subscribe(_ => blockController?.AddCrewToWork()).AddTo(disposables);
        removeCrewToWork.OnClickAsObservable().Subscribe(_ => blockController?.RemoveCrewFromWork()).AddTo(disposables);
        addCrewToRest.OnClickAsObservable().Subscribe(_ => blockController?.AddCrewToRest()).AddTo(disposables);
        removeCrewToRest.OnClickAsObservable().Subscribe(_ => blockController?.RemoveCrewFromRest()).AddTo(disposables);

        // Подписываемся на изменения количества персонала для обновления UI
        blockController?.crewAtWork.Subscribe(count => currentCrewAtWork.text = $"Currently working: {count}").AddTo(disposables);
        blockController?.crewAtRest.Subscribe(count => currentCrewAtRest.text = $"Currently resting: {count}").AddTo(disposables);
        blockController?.crewAtIdle.Subscribe(count => currentCrewAtIdle.text = $"Currently idling: {count}").AddTo(disposables);

        // Начальное обновление UI
        UpdateCrewCounts();
    }

    private void UpdateCrewCounts()
    {
        if (blockController != null)
        {
            currentCrewAtWork.text = $"Currently working: {blockController.crewAtWork.Value}";
            currentCrewAtRest.text = $"Currently resting: {blockController.crewAtRest.Value}";
            currentCrewAtIdle.text = $"Currently idling: {blockController.crewAtIdle.Value}";
        }
        else
        {
            currentCrewAtWork.text = "N/A";
            currentCrewAtRest.text = "N/A";
            currentCrewAtIdle.text = "N/A";
        }
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }
}