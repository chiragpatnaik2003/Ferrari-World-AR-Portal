using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class LowestPlanePlacement : MonoBehaviour
{
    public GameObject xrOrigin; // Assign XR Origin that contains ARPlaneManager & ARRaycastManager
    public GameObject objectToInstantiate; // Prefab to instantiate

    private ARPlaneManager planeManager;
    private ARRaycastManager raycastManager;
    private ARPlane lowestPlane = null;

    void Start()
    {
        // Get AR Managers from XR Origin
        planeManager = xrOrigin.GetComponent<ARPlaneManager>();
        raycastManager = xrOrigin.GetComponent<ARRaycastManager>();

        if (planeManager != null)
            planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDestroy()
    {
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        float minY = float.MaxValue;
        foreach (var plane in planeManager.trackables) {
            if (plane.alignment == PlaneAlignment.HorizontalUp && plane.transform.position.y < minY) {
                minY = plane.transform.position.y;
                lowestPlane = plane;
            }
        }
    }

    void Update()
    {
        if (lowestPlane == null || Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began)
            return;

        Touch touch = Input.GetTouch(0);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon)) {
            foreach (var hit in hits) {
                if (hit.trackableId == lowestPlane.trackableId) {
                    Instantiate(objectToInstantiate, hit.pose.position, Quaternion.identity);
                    break;
                }
            }
        }
    }
}
