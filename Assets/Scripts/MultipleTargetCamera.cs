using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    public static MultipleTargetCamera instance;
    public List<Transform> targets;
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothTime = .5f;


    
    public float maxZoomHorizontal = 10f;
    public float minZoomHorizontal = 4f;
    public float maxZoomVertical = 17f;
    public float minZoomVertical = 4f;
    private float zoomLimiter = 15f;

    private Camera cam;
    private Vector3 velocity;

    public void Awake()
    {
        if(instance != null)
            Destroy(instance.gameObject);
        instance = this;

        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
            return;

        Move();
        Zoom();        
    }

    void Move()
    {
        Vector3 newPosition = GetCenterPoint() + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);


    }

    void Zoom()
    {
        Vector2 greatestDistance = GetGreatestDistance();
        float newZoom = cam.orthographicSize;
        if (greatestDistance.x > greatestDistance.y)
        {
            zoomLimiter = maxZoomHorizontal + minZoomHorizontal;
            newZoom = Mathf.Lerp(minZoomHorizontal, maxZoomHorizontal, greatestDistance.x / zoomLimiter);
        }
        else
        {
            zoomLimiter = maxZoomVertical + minZoomVertical;
            newZoom = Mathf.Lerp(minZoomVertical, maxZoomVertical, greatestDistance.y / zoomLimiter);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    Vector2 GetGreatestDistance()
    {
        if (targets.Count <= 0)
            return Vector2.zero;
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
            bounds.Encapsulate(targets[i].position);
        return bounds.size;
    }

    Vector3 GetCenterPoint()
    {

        //When everyone is dead goes to this position
        if(targets.Count <= 0)
            return Vector3.zero;

        //if there is only one player, follow them
        if (targets.Count == 1)
            return targets[0].position;

        //when targets.Count > 1
        Bounds bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
            bounds.Encapsulate(targets[i].position);
        return bounds.center;
    }
}
