using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class RobotManager : MonoBehaviour
{
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARRaycastManager raycastManager;

    public GameObject robot;
    private GameObject spawnedObject;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!TryGetTouchPosition(out var touchPosition)) { return; }
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(robot, hitPose.position, hitPose.rotation);
            }
        }
    }

    public void Reset()
    {
        Destroy(spawnedObject);
    }
}
