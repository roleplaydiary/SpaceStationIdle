using System.Linq;
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

    private void LoadMajorEvent()
    {
        var stationBlockData = ServiceLocator.Get<StationController>().StationData.DepartmentData
            .FirstOrDefault(block => block.Key == blockController.GetBlockType());

        if (stationBlockData.Value.BlockEvent > 0)
        {
            StationMajorEventType eventType = (StationMajorEventType)stationBlockData.Value.BlockEvent;
            Debug.Log($"{name} has major event {eventType}");
            //EventStart(eventType);
            MajorEventData eventData = new MajorEventData()
            { 
                StationMajorEventType = eventType, 
                Department = blockController.GetBlockType() 
            };
            ServiceLocator.Get<StationEventsController>().OnMajorEventStarted.OnNext(eventData);
        }
    }

    private void Initialize()
    {
        ServiceLocator.Get<StationEventsController>().OnMajorEventStarted.Subscribe(majorEventData =>
        {
            Debug.Log("Major event controller: Initialize: OnMajorEventStarted");
            var department = blockController.GetBlockType();
            if (majorEventData.Department == department)
            {
                EventStart(majorEventData.StationMajorEventType);
                blockController.StartBlockEvent(majorEventData.StationMajorEventType);
            }
        }).AddTo(this);
        
        
        LoadMajorEvent();//Загружаем после инициализации, чтобы ивент вызвался при загрузке
    }

    protected virtual void EventStart(StationMajorEventType eventType)
    {
        majorEventsHandler.Initialize(eventType);
        blockController.RemoveCrewFromWork();//TODO: Убрать всех работающих сотрудников и перевести в режим паники
    }
}
