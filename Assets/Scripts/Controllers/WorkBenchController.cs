using UnityEngine;

public class WorkBenchController : MonoBehaviour
{
    [SerializeField] private Transform workPosition;
    [SerializeField] private WorkBenchResource producedResource;
    [SerializeField] private float productionRate;
    [SerializeField] private float energyConsumptionRate; // Потребление энергии в единицу времени (например, в секунду)

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetWorkPosition()
    {
        return workPosition.position;
    }

    public WorkBenchResource ProducedResource => producedResource;
    public float ProductionRate => productionRate;
    public float EnergyConsumptionRate => energyConsumptionRate;
}