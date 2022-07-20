using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public Transform MapTransform;
    private Camera _camera;
    private readonly float _minCameraScale = 0.5f;
    private readonly float _maxCameraScale = 5f;
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

            difference.x = Mathf.Abs(difference.x) < 0.1 ? 0 : difference.x;
            difference.y = Mathf.Abs(difference.y) < 0.1 ? 0 : difference.y;

            if (difference.sqrMagnitude < 0.1f)
            {
                return;
            }

            _camera.transform.position += difference;
        }
    }

    void ZoomCamera(float scrollValue)
    {
        var tileScale = MapTransform.localScale;
        var scaleChange = scrollValue * 0.05f * tileScale.x;
        var newTileScale = Mathf.Clamp(scaleChange + tileScale.x, _minCameraScale, _maxCameraScale);

        MapTransform.localScale = new Vector3(newTileScale, newTileScale, -10);

        // we have to calculate the real values becuase of clamping
        var cameraPosScale = newTileScale / tileScale.x;
        float cameraX = _camera.transform.position.x;
        float cameraY = _camera.transform.position.y;

        _camera.transform.position = new Vector3(cameraX * cameraPosScale, cameraY * cameraPosScale, -10);
    }
}
