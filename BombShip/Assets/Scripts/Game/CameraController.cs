using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Camera informations")]
    public Vector3 offset;
    public float smoothTime = .5f;

    [Header("Camera bounds")]
    public bool enableBounds;
    public Vector3 minCameraPosition;
    public Vector3 maxCameraPosition;

    [Header("Camera zoom")]
    public float minZoom = 4f;
    public float maxZoom = 4.5f;
    public float zoomLimiter = 12f;

    private Camera cam;
    private Vector3 velocity;

    private void Start() {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate() {
        if (GameController.gc.targets == null || GameController.gc.targets.Count == 0) {
            return;
        }

        Move();
        Zoom();
    }

    private void Move() {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        if (enableBounds) {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(Mathf.Clamp(newPosition.x, minCameraPosition.x, maxCameraPosition.x), Mathf.Clamp(newPosition.y, minCameraPosition.y, maxCameraPosition.y), Mathf.Clamp(newPosition.z, minCameraPosition.z, maxCameraPosition.z)), ref velocity, smoothTime);
        } else {
            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        }
    }

    private void Zoom() {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    private Vector3 GetCenterPoint() {
        if (GameController.gc.targets.Count == 1) {
            return GameController.gc.targets[0].transform.position;
        }

        var bounds = new Bounds(GameController.gc.targets[0].transform.position, Vector3.zero);

        for (int i = 0; i < GameController.gc.targets.Count; i++) {
            bounds.Encapsulate(GameController.gc.targets[i].transform.position);
        }

        return bounds.center;
    }

    private float GetGreatestDistance() {
        var bounds = new Bounds(GameController.gc.targets[0].transform.position, Vector3.zero);

        for (int i = 0; i < GameController.gc.targets.Count; i++) {
            bounds.Encapsulate(GameController.gc.targets[i].transform.position);
        }

        return bounds.size.x;
    }
}
