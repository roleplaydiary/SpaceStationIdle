using UnityEngine;

public class WorkBenchController : MonoBehaviour
{
    [SerializeField] private Transform workPosition;
    [SerializeField] private float productionRate;
    [SerializeField] private float energyConsumptionRate;
    [SerializeField] private float moodConsumptionRate;

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetWorkPosition()
    {
        return workPosition.position;
    }

    public float ProductionRate => productionRate;
    public float EnergyConsumptionRate => energyConsumptionRate;
    public float MoodConsumptionRate => moodConsumptionRate;
}