using System.Collections.Generic;
using Controllers;
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

        // Проверяем, есть ли уже активные бонусы
        if (bonusObjects.Count > 0)
        {
            Debug.Log("Уже есть активные бонусы. Удаляем их.");
            // Удаляем все существующие бонусные объекты
            foreach (var bonusObject in bonusObjects.ToArray()) // Используем ToArray, чтобы избежать проблем с изменением коллекции во время итерации
            {
                if (bonusObject != null)
                {
                    Destroy(bonusObject);
                    PlaySound("error_sound");
                }
            }
            bonusObjects.Clear(); // Очищаем список отслеживаемых бонусов
        }
        else
        {
            // Если бонусов нет, создаем новый
            var bonusType = BonusTypeRandomize();
            bonusType = BonusEventType.Credits; //TODO: FOR TESTS, DELETE AFTER!
            BonusInitialization(bonusType);
        }
    }

    private void PlaySound(string soundName)
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        audioManager.PlayUISound(audioManager.GetUISound(soundName));
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
        
        PlaySound("bonus_spawn");
        //подписаться на бонус и удалить его из списка, когда его подберут
        // Получаем компонент BonusPickup у созданного объекта
        var bonusPickup = spawnedBonus.GetComponent<BonusPickup>();

        if (bonusPickup != null)
        {
            // Подписываемся на событие OnBonusPickedUp этого конкретного бонуса
            bonusPickup.OnBonusPickedUp
                .Where(pickedUp => pickedUp) // Фильтруем, чтобы обработать только true
                .First() // Берем только первое срабатывание (один раз подобрали - удаляем)
                .Subscribe(_ =>
                {
                    // Вызываем метод выдачи награды
                    //GrantReward(BonusEventType.Credits, amount);
                    var playerController = ServiceLocator.Get<PlayerController>();
                    playerController.AddCredits(amount);
                    Debug.Log("Игрок получил бонус в размере " + amount);
                    
                    var audioManager = ServiceLocator.Get<AudioManager>();
                    audioManager.PlayUISound(audioManager.GetUISound("reward_obtain"));
                    // Удаляем объект бонуса
                    if (spawnedBonus != null) // Проверка на случай, если объект уже был уничтожен
                    {
                        Destroy(spawnedBonus);
                        bonusObjects.Remove(spawnedBonus); // Удаляем из списка отслеживаемых бонусов
                    }
                })
                .AddTo(spawnedBonus); // Важно привязать подписку к жизненному циклу бонуса
        }
        else
        {
            Debug.LogError("BonusPickup компонент не найден на созданном Credit Bonus!");
        }
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
