using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minFOV = 40;
    [SerializeField] private float maxFOV = 80f;
    [SerializeField] private float deceleration = 5f; // Скорость замедления

    private Camera cam;
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private float initialPinchDistance;
    private bool isDragging; // Добавляем флаг для отслеживания перетаскивания
    private Vector3 velocity; // Скорость движения камеры

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Для ПК (мышь)
        if (Input.GetMouseButtonDown(0))
        {
            // Проверяем, не находится ли мышь над UI
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                isDragging = true;
                dragStartPosition = Input.mousePosition;
                dragCurrentPosition = dragStartPosition;
            }
            else
            {
                isDragging = false; // Если над UI, не начинаем перетаскивание камеры
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            dragCurrentPosition = Input.mousePosition;
            Vector3 moveDelta = dragStartPosition - dragCurrentPosition;

            cam.transform.position += new Vector3(moveDelta.x * moveSpeed * Time.deltaTime, 0, moveDelta.y * moveSpeed * Time.deltaTime);
            dragStartPosition = dragCurrentPosition;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newFOV = cam.fieldOfView - scroll * zoomSpeed;
        cam.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);

        // Перемещение камеры (тач)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // Проверяем, не находится ли касание над UI
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    dragStartPosition = touch.position;
                    dragCurrentPosition = dragStartPosition;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                // Перемещаем камеру только если касание началось не над UI
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    dragCurrentPosition = touch.position;
                    Vector3 moveDelta = dragStartPosition - dragCurrentPosition;

                    cam.transform.position += new Vector3(moveDelta.x * moveSpeed * Time.deltaTime, 0, moveDelta.y * moveSpeed * Time.deltaTime);
                    dragStartPosition = dragCurrentPosition;
                }
            }
        }
        // Приближение/отдаление (тач)
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Проверяем, что ни одно из касаний не начиналось над UI
            bool isOverUI = EventSystem.current.IsPointerOverGameObject(touchZero.fingerId) || EventSystem.current.IsPointerOverGameObject(touchOne.fingerId);

            if ((touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began) && !isOverUI)
            {
                initialPinchDistance = Vector2.Distance(touchZero.position, touchOne.position);
            }
            else if ((touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved) && initialPinchDistance > 0 && !isOverUI)
            {
                float currentPinchDistance = Vector2.Distance(touchZero.position, touchOne.position);
                float pinchDelta = currentPinchDistance - initialPinchDistance;

                float newFOVTouch = cam.fieldOfView - pinchDelta * zoomSpeed * Time.deltaTime;
                cam.fieldOfView = Mathf.Clamp(newFOVTouch, minFOV, maxFOV);

                initialPinchDistance = currentPinchDistance;
            }
        }
    }
}