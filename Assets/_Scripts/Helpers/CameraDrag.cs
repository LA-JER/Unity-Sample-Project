using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    private Vector3 dragOrigin;
    private bool isDragging = false;

    // Optional boundaries
    [SerializeField] private float sensitivity = 1.5f;
    [SerializeField] private float minX = -10f, maxX = 10f;
    [SerializeField] private float minY = -10f, maxY = 10f;

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
    }

    void DragCamera()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 difference = Camera.main.ScreenToViewportPoint(dragOrigin - currentMousePosition);

        Vector3 move = new Vector3(difference.x * sensitivity, difference.y * sensitivity, 0);
        Camera.main.transform.Translate(move, Space.World);

        // Clamp camera position to defined boundaries
        Camera.main.transform.position = new Vector3(
            Mathf.Clamp(Camera.main.transform.position.x, minX, maxX),
            Mathf.Clamp(Camera.main.transform.position.y, minY, maxY),
            Camera.main.transform.position.z
        );

        dragOrigin = currentMousePosition;
    }
}
