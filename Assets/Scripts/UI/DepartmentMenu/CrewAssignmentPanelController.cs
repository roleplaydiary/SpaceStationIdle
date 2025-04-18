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
        blockController?.crewAtWork.Subscribe(count => currentCrewAtWork.text = count.ToString()).AddTo(disposables);
        blockController?.crewAtRest.Subscribe(count => currentCrewAtRest.text = count.ToString()).AddTo(disposables);
        blockController?.crewAtIdle.Subscribe(count => currentCrewAtIdle.text = count.ToString()).AddTo(disposables);

        // Начальное обновление UI
        UpdateCrewCounts();
    }

    private void UpdateCrewCounts()
    {
        if (blockController != null)
        {
            currentCrewAtWork.text = blockController.crewAtWork.Value.ToString();
            currentCrewAtRest.text = blockController.crewAtRest.Value.ToString();
            currentCrewAtIdle.text = blockController.crewAtIdle.Value.ToString();
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