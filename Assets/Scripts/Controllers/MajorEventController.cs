using Controllers;
using UniRx;
using UnityEngine;

public class MajorEventController : MonoBehaviour
{
    [SerializeField] private MajorEventsHandler majorEventsHandler;
    private StationBlockController blockController;
    private void Start()
    {
        blockController = GetComponent<StationBlockController>();
        if (blockController == null)
        {
            Debug.LogError($"{name} doesn't have a StationBlockController!");
            return;
        }
        var gameController = ServiceLocator.Get<GameController>();
        gameController.OnGameInitialized
            .Where(initialized => initialized)
            .Subscribe(_ => Initialize())
            .AddTo(this);
    }

    private void Initialize()
    {
        ServiceLocator.Get<StationEventsController>().OnMajorEventStarted.Subscribe(majorEventData =>
        {
            var department = blockController.GetBlockType();
            if (majorEventData.Department == department)
            {
                EventStart(majorEventData.StationMajorEventType);
            }
        }).AddTo(this);
    }

    protected virtual void EventStart(StationMajorEventType eventType)
    {
        majorEventsHandler.Initialize(eventType);
        blockController.RemoveCrewFromWork();
    }
}
