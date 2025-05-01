using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BonusEventController : MonoBehaviour
{
    [SerializeField] private Transform bonusPositionParent;
    private List<Transform> bonusPositions = new List<Transform>();
    
    [SerializeField] private GameObject creditBonusPrefab;
    [SerializeField] private GameObject rpBonusPrefab;
    [SerializeField] private GameObject metallBonusPrefab;
    
    private List<GameObject> bonusObjects = new List<GameObject>(); // TODO: удаление бонусов по таймеру
    
    private void Start()
    {
        var gameController = ServiceLocator.Get<GameController>();
        gameController.OnGameInitialized
            .Where(initialized => initialized)
            .Subscribe(_ => Initialize())
            .AddTo(this);
    }

    private void Initialize()
    {
        BonusPositionListInitialize();
        
        ServiceLocator.Get<StationEventsController>().OnEventStarted.Subscribe(value =>
        {
            if (value == StationEventType.BonusEvent)
            {
                BonusEventStart();
            }
        }).AddTo(this);
    }

    private void BonusPositionListInitialize()
    {
        foreach (Transform position in bonusPositionParent)
        {
            bonusPositions.Add(position);
        }
    }

    private void BonusEventStart()
    {
        Debug.Log("BonusEventController: Bonus Event Start");
        var audioManager = ServiceLocator.Get<AudioManager>();
        audioManager.PlayUISound(audioManager.GetUISound("bonus_spawn"));
        var bonus = BonusTypeRandomize();
        bonus = BonusEventType.Credits;//TODO: FOR TESTS, DELETE AFTER!
        BonusInitialization(bonus);
    }

    private void BonusInitialization(BonusEventType bonusEventType)
    {
        switch (bonusEventType)
        {
            case BonusEventType.Credits:
                BonusCreditSpawn();
                break;
            case BonusEventType.ResearchPoints:
                BonusRPSpawn();
                break;
            case BonusEventType.Resource:
                BonusResourceSpawn();
                break;
        }
    }

    private BonusEventType BonusTypeRandomize()
    {
        var random = Random.Range(0, 100);
        if (random < 45)
        {
            return BonusEventType.Credits;
        }
        else if (random < 90)
        {
            return BonusEventType.ResearchPoints;
        }
        else if (random < 100)
        {
            return BonusEventType.Resource;
        }

        return BonusEventType.Credits;
    }

    private void BonusCreditSpawn()
    {
        var amount = Random.Range(1, 10);
        var spawnedBonus = Instantiate(creditBonusPrefab);
        spawnedBonus.transform.position = GetRandomBonusPosition();
        bonusObjects.Add(spawnedBonus);
        //подписаться на бонус и удалить его из списка, когда его подберут
    }
    
    private void BonusRPSpawn()
    {
        var amount = Random.Range(1, 10);
    }
    
    private void BonusResourceSpawn()
    {
        float amount = Random.Range(0.1f, 0.5f);
    }

    private Vector3 GetRandomBonusPosition()
    {
        var randomPosition = Random.Range(0, bonusPositions.Count);
        return bonusPositions[randomPosition].position;
    }
}

public enum BonusEventType
{
    Credits,
    ResearchPoints,
    Resource
}
