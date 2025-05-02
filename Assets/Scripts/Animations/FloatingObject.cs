using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Tooltip("Скорость вращения вокруг оси Y (градусов в секунду)")]
    [SerializeField] private float rotationSpeed = 30f;

    [Tooltip("Амплитуда колебания по оси Y")]
    [SerializeField] private float floatAmplitude = 0.15f;

    [Tooltip("Скорость колебания по оси Y")]
    [SerializeField] private float floatSpeed = 1f;

    private Vector3 initialPosition;
    private float timeOffset;

    private void Start()
    {
        initialPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        // Добавляем случайное смещение по времени, чтобы объекты парили не синхронно
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        // Вращение вокруг оси Y
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Колебание по оси Y (синусоидальное движение)
        float newY = initialPosition.y + Mathf.Sin(Time.time * floatSpeed + timeOffset) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}