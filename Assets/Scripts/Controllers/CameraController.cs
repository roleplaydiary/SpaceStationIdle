using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 5f;
    private float minFOV = 40;
    private float maxFOV = 80f;

    private Camera cam;
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private float initialPinchDistance;
    private bool isDragging; // Добавляем флаг для отслеживания перетаскивания

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Для ПК (мышь)
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartPosition = Input.mousePosition;
            dragCurrentPosition = dragStartPosition;
            Debug.Log("Нажатие ЛКМ: dragStartPosition = " + dragStartPosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            dragCurrentPosition = Input.mousePosition;
            Vector3 moveDelta = dragStartPosition - dragCurrentPosition;

            Debug.Log("dragCurrentPosition = " + dragCurrentPosition + ", moveDelta = " + moveDelta);
            cam.transform.position += new Vector3(moveDelta.x * moveSpeed * Time.deltaTime, 0, moveDelta.y * moveSpeed * Time.deltaTime); // Перемещаем камеру в обратном направлении
            Debug.Log("Позиция камеры: " + cam.transform.position);
            dragStartPosition = dragCurrentPosition;
        }
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newFOV = cam.fieldOfView - scroll * zoomSpeed;
        cam.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV); // Изменяем fieldOfView

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

                cam.transform.position += new Vector3(moveDelta.x * moveSpeed * Time.deltaTime, 0, moveDelta.y * moveSpeed * Time.deltaTime);
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

                float newFOVTouch = cam.fieldOfView - pinchDelta * zoomSpeed * Time.deltaTime;
                cam.fieldOfView = Mathf.Clamp(newFOVTouch, minFOV, maxFOV);

                initialPinchDistance = currentPinchDistance;
            }
        }
        // else // Для ПК (мышь)
        // {
        //     if (Input.GetMouseButtonDown(0)) // Нажатие ЛКМ
        //     {
        //         Debug.Log("CAMERA CONTROLLER INPUT.GetMouseButtonDown НАЖАТИЕ");
        //         isDragging = true;
        //         dragStartPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        //     }
        //
        //     if (Input.GetMouseButtonUp(0)) // Отпускание ЛКМ
        //     {
        //         Debug.Log("CAMERA CONTROLLER INPUT.GetMouseButtonDown ОТПУСКАНИЕ");
        //         isDragging = false;
        //     }
        //
        //     if (isDragging) // Перемещение при нажатой ЛКМ
        //     {
        //         Vector3 dragCurrentPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        //         Vector3 moveDelta = dragStartPosition - dragCurrentPosition;
        //
        //         cam.transform.position += moveDelta;
        //     }
        //
        //     float scroll = Input.GetAxis("Mouse ScrollWheel");
        //     cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom); 
        // }
    }
}