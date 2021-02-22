using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    [SerializeField]
    new private Camera camera;

    [SerializeField]
    private KeyCode dragKey = KeyCode.Mouse1;

    [SerializeField]
    private float zoomSensitivity = 0.1f;

    [SerializeField]
    private float minZoom = 2.0f;

    [SerializeField]
    private float maxZoom = 10.0f;

    private bool isDragging;
    private Vector2 dragOffset = Vector2.zero;

    private Vector2 lastMouseScreenPos;

    private float currentZoom = 0.3f;
    private float targetZoom = 0.3f;

    private Vector3 lastZoomMousePos;

    public bool IsDragging()
    {
        return isDragging;
    }

    private void Start()
    {
        Vector3 mousePosRaw = Input.mousePosition;
        Vector3 screenMousePosition = new Vector3(Mathf.Clamp(mousePosRaw.x, 0.0f, Screen.width), Mathf.Clamp(mousePosRaw.y, 0.0f, Screen.height));
        lastMouseScreenPos = screenMousePosition / new Vector2(Screen.width, Screen.height);

        float zoomValue = Mathf.Lerp(minZoom, maxZoom, targetZoom);
        camera.orthographicSize = zoomValue;
    }

    private void Update()
    {
        Vector3 mousePosRaw = Input.mousePosition;
        Vector2 rawMousePos = new Vector2(Mathf.Clamp(mousePosRaw.x, 0.0f, Screen.width), Mathf.Clamp(mousePosRaw.y, 0.0f, Screen.height));
        Vector2 currentMouseScreenPos = rawMousePos / new Vector2(Screen.width, Screen.height);

        Vector2 camViewportDim = new Vector2(camera.orthographicSize * camera.aspect * 2.0f, camera.orthographicSize * 2.0f);

        Vector2 mouseScreenDelta = currentMouseScreenPos - lastMouseScreenPos;
        Vector2 mouseWorldDelta = camViewportDim * mouseScreenDelta;

        if (Input.GetKey(dragKey))
        {
            float threshold = Desk.DragThresholdPx;
            var dragThresholdNormalizedSqrMag = Vector2.SqrMagnitude(new Vector2(threshold, threshold) / new Vector2(Screen.width, Screen.height));

            dragOffset += mouseScreenDelta;

            if (dragOffset.sqrMagnitude > dragThresholdNormalizedSqrMag)
            {
                isDragging = true;
            }

            if (isDragging)
            {
                camera.transform.position -= new Vector3(mouseWorldDelta.x, mouseWorldDelta.y, 0.0f);
            }
        }

        // Zoom

        Vector2 scrollDelta = Input.mouseScrollDelta;
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePosWorldBeforeZoom = camera.ScreenToWorldPoint(mousePosScreen);

        Vector2 zoomDelta = scrollDelta * zoomSensitivity;
        targetZoom = Mathf.Clamp(targetZoom - zoomDelta.y, 0.00f, 1.0f);
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, 1.0f);

        float zoomValue = Mathf.Lerp(minZoom, maxZoom, currentZoom);
        camera.orthographicSize = zoomValue;

        Vector3 mousePosWorldAfterZoom = camera.ScreenToWorldPoint(mousePosScreen);
        Vector3 zoomOffset = mousePosWorldBeforeZoom - mousePosWorldAfterZoom;


        camera.transform.position += zoomOffset;

        lastMouseScreenPos = currentMouseScreenPos;
    }

    private void LateUpdate()
    {
        if (!Input.GetKey(dragKey))
        {
            isDragging = false;
            dragOffset = Vector2.zero;
        }
    }
}
