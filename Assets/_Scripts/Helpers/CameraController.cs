using System.Xml.Serialization;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 dragOrigin;
    private bool isDragging = false;

    [SerializeField] private float minX = -10f, maxX = 10f;
    [SerializeField] private float minY = -10f, maxY = 10f;
    public float targetCameraSize = 10;
    public float mouseDragSpeed = 1.5f;
    public float touchDragSpeed = 0.1f;
    public float mouseZoomSpeed = 5.0f;
    public float touchZoomSpeed = 5.0f;
    public float minZoom = 8.0f;
    private float maxZoom = 11.0f;

    private Vector2 previousTouchPosition1;
    private Vector2 previousTouchPosition2;
    private float previousTouchDistance;
    private float targetXPOSReach;
    private float targetXNegReach;
    private float targetYPOSReach;
    private float targetYNegReach;
    
    public float zoomFactor;

    private void Start()
    {
        maxZoom = targetCameraSize;
        targetXPOSReach = targetCameraSize * Camera.main.aspect + maxX;
        targetXNegReach = -targetCameraSize * Camera.main.aspect + minX;
        targetYPOSReach = targetCameraSize + maxY;
        targetYNegReach = -targetCameraSize + minY;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            DragCamera();
        }
        HandleTouchInput();
        HandleMouseScrollZoom();
        ClampCameraPosition();
    }

    void DragCamera()
    {
        float adjustedDragSpeed = (targetCameraSize / Camera.main.orthographicSize) * mouseDragSpeed;

        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - currentMousePosition);

        Vector3 move = new Vector3(difference.x * adjustedDragSpeed, difference.y * adjustedDragSpeed, 0);
        Camera.main.transform.Translate(move, Space.World);



        // Clamp camera position to defined boundaries
        float yReach = Mathf.Abs(Camera.main.transform.position.y) + Camera.main.orthographicSize;
        float xReach = Mathf.Abs(Camera.main.transform.position.x) + Camera.main.orthographicSize * Camera.main.aspect;

        Camera.main.transform.position = new Vector3(
            Mathf.Clamp(Camera.main.transform.position.x, targetXNegReach + xReach, targetXPOSReach - xReach),
            Mathf.Clamp(Camera.main.transform.position.y, targetYNegReach + yReach, targetYPOSReach - yReach),
            Camera.main.transform.position.z
        );

        dragOrigin = currentMousePosition;
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Check for the phase of the touches
            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                Vector2 touchPosition1 = touch1.position;
                Vector2 touchPosition2 = touch2.position;

                // Calculate the movement delta
                Vector2 touchDelta1 = touchPosition1 - previousTouchPosition1;
                Vector2 touchDelta2 = touchPosition2 - previousTouchPosition2;

                // Average the deltas to get the overall movement
                Vector2 averageDelta = (touchDelta1 + touchDelta2) / 2;

                //make the drag speed faster if zoomed in
                float adjustedDragSpeed = (targetCameraSize / Camera.main.orthographicSize) * touchDragSpeed;


                // Move the camera based on the average delta
                Vector3 move = new Vector3(-averageDelta.x * adjustedDragSpeed, -averageDelta.y * adjustedDragSpeed, 0);
                Camera.main.transform.Translate(move, Space.World);


                // Calculate the distance between the two touches
                float currentTouchDistance = Vector2.Distance(touchPosition1, touchPosition2);

                // Calculate the zoom factor
                 zoomFactor =  ( 1 / (previousTouchDistance / currentTouchDistance));

                // Apply the zoom to the camera's orthographic size
                Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize * (  zoomFactor )* touchZoomSpeed, minZoom, maxZoom);

                // Update the previous touch positions
                previousTouchPosition1 = touchPosition1;
                previousTouchPosition2 = touchPosition2;
                //previousTouchDistance = currentTouchDistance;

            }
            else if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                // Initialize previous positions on touch begin
                previousTouchPosition1 = touch1.position;
                previousTouchPosition2 = touch2.position;

                previousTouchDistance = Vector2.Distance(touch1.position, touch2.position);
            }
        }
    }

    void HandleMouseScrollZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * mouseZoomSpeed, minZoom, maxZoom);
        }
    }

    void ClampCameraPosition()
    {
        float verticalExtent = Camera.main.orthographicSize;
        float horizontalExtent = Camera.main.orthographicSize * Camera.main.aspect;

        float minX = targetXNegReach + horizontalExtent;
        float maxX = targetXPOSReach - horizontalExtent;
        float minY = targetYNegReach + verticalExtent;
        float maxY = targetYPOSReach - verticalExtent;

        Vector3 pos = Camera.main.transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        Camera.main.transform.position = pos;
    }
}
