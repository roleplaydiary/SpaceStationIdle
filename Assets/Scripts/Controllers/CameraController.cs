using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minFOV = 40;
    [SerializeField] private float maxFOV = 80f;
    [SerializeField] private float deceleration = 5f; // Скорость замедления
    
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private float initialPinchDistance;
    private bool isDragging; // Добавляем флаг для отслеживания перетаскивания
    private Vector3 velocity; // Скорость движения камеры

    private UIController uiController;
    void Start()
    {
        uiController = ServiceLocator.Get<UIController>();
    }

    void Update()
    {
        // Для ПК (мышь)
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartPosition = Input.mousePosition;
            dragCurrentPosition = dragStartPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            if (!uiController)
            {
                return;
            }
            if (uiController.IsAnyUIOpen)
            {
                return;
            }
            
            dragCurrentPosition = Input.mousePosition;
            Vector3 moveDelta = dragStartPosition - dragCurrentPosition;

            gameCamera.transform.position += new Vector3(moveDelta.x * moveSpeed * Time.deltaTime, 0, moveDelta.y * moveSpeed * Time.deltaTime);
            dragStartPosition = dragCurrentPosition;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newFOV = gameCamera.fieldOfView - scroll * zoomSpeed;
        gameCamera.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);

        // Перемещение камеры (тач)
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                dragStartPosition = touch.position;
                dragCurrentPosition = dragStartPosition;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                dragCurrentPosition = touch.position;
                Vector3 moveDelta = dragStartPosition - dragCurrentPosition;

                gameCamera.transform.position += new Vector3(moveDelta.x * moveSpeed * Time.deltaTime, 0, moveDelta.y * moveSpeed * Time.deltaTime);
                dragStartPosition = dragCurrentPosition;
            }
        }
        // Приближение/отдаление (тач)
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialPinchDistance = Vector2.Distance(touchZero.position, touchOne.position);
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                float currentPinchDistance = Vector2.Distance(touchZero.position, touchOne.position);
                float pinchDelta = currentPinchDistance - initialPinchDistance;

                float newFOVTouch = gameCamera.fieldOfView - pinchDelta * zoomSpeed * Time.deltaTime;
                gameCamera.fieldOfView = Mathf.Clamp(newFOVTouch, minFOV, maxFOV);

                initialPinchDistance = currentPinchDistance;
            }
        }
    }
}