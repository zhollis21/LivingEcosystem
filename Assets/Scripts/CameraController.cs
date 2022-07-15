using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;
    private int _minCameraZoom = 1;
    private int _maxCameraZoom = 100;
    private Vector3 _mouseDragOrigin;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (scrollDelta != Vector2.zero)
        {
            ZoomCamera(scrollDelta.y);
        }

        // If we just pressed middle mouse store the origin
        if (Input.GetMouseButtonDown(2))
        {
            _mouseDragOrigin = _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        // While holding down middle mouse move the camera
        if (Input.GetMouseButton(2))
        {
            Vector3 difference = _mouseDragOrigin - _camera.ScreenToWorldPoint(Input.mousePosition);

            _camera.transform.position += difference;
        }
    }

    void ZoomCamera(float scrollValue)
    {
        float newSize = _camera.orthographicSize - scrollValue;

        // Make sure the new zoom is within the bounds of the min and max
        _camera.orthographicSize = Mathf.Clamp(newSize, _minCameraZoom, _maxCameraZoom);
    }
}
