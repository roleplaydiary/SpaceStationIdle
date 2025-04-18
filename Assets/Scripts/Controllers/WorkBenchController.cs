using UnityEngine;

public class WorkBenchController : MonoBehaviour
{
    [SerializeField] private Transform workPosition;
    [SerializeField] private WorkBenchResource producedResource; // Тип производимого ресурса (например, "Кредит", "Энергия")
    [SerializeField] private float productionRate;   // Количество производимого ресурса в минуту

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
}