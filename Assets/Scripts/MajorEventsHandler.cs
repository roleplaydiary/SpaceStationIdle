using UnityEngine;

public class MajorEventsHandler : MonoBehaviour
{
    [SerializeField] private GameObject fireHazardState;
    [SerializeField] private GameObject asteroidHitState;
    [SerializeField] private GameObject aliensState;
    [SerializeField] private GameObject epidemicState;
    [SerializeField] private GameObject animalyState;

    public void Initialize(StationMajorEventType eventType)
    {
        switch (eventType)
        {
            case StationMajorEventType.AsteroidHit:
                if(asteroidHitState)
                    asteroidHitState.SetActive(true);
                break;
            case StationMajorEventType.FireHazard:
                if(fireHazardState)
                    fireHazardState.SetActive(true);
                break;
            case StationMajorEventType.Aliens:
                if(aliensState)
                    aliensState.SetActive(true);
                break;
            case StationMajorEventType.Epidemic:
                if(epidemicState)
                    epidemicState.SetActive(true);
                break;
            case StationMajorEventType.Anomaly:
                if(animalyState)
                    animalyState.SetActive(true);
                break;
            case StationMajorEventType.None:
                DisableAllEvents();
                break;
            default:
                DisableAllEvents();
                break;
        }
    }

    private void DisableAllEvents()
    {
        if(asteroidHitState)
            asteroidHitState.SetActive(false);
        if(fireHazardState)
            fireHazardState.SetActive(false);
        if(aliensState)
            aliensState.SetActive(false);
        if(epidemicState)
            epidemicState.SetActive(false);
        if(animalyState)
            animalyState.SetActive(false);
    }
}
