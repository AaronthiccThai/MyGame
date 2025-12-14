using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera mainCamera;
    public float zoomSpeed = 0.1f;
    public float minZoom = 0.5f;
    public float maxZoom = 1f;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            ZoomIn();
        }
        else if (scroll < 0f)
        {
            ZoomOut();
        }

    }

    private void ZoomIn()
    {
        if (mainCamera.orthographicSize > minZoom)
        {
            mainCamera.orthographicSize = Mathf.Max(mainCamera.orthographicSize - zoomSpeed, minZoom);
        }
    }

    private void ZoomOut()
    {
        if (mainCamera.orthographicSize < maxZoom)
        {
            mainCamera.orthographicSize = Mathf.Min(mainCamera.orthographicSize + zoomSpeed, maxZoom);
        }
    }
}
