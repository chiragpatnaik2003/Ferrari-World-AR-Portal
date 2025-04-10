using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PortalSpawner : MonoBehaviour
{
    public GameObject prefabPortal; // The portal prefab
    private GameObject spawnedPortal; // Reference to the spawned portal

    public ARRaycastManager raycastManager; // AR Raycast Manager for plane detection
    public ARPlaneManager planeManager; // AR Plane Manager for plane tracking

    public Button togglePlaneButton; // Button to toggle plane detection

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>(); // List to store raycast hits
    private bool planesEnabled = true; // Toggle state for plane detection
    private bool hasSpawned = false; // Prevent multiple spawns
    public float heightOffset = 0.1f; // Height offset for better placement

    void Start()
    {
        // Add listener to the button for toggling plane detection
        togglePlaneButton.onClick.AddListener(TogglePlaneDetection);
    }

    void Update()
    {
        // Ignore touches over UI elements
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return;
        }

        // Handle single touch input only if the portal has not been spawned
        if (Input.touchCount == 1 && !hasSpawned)
        {
            HandleSingleTouch();
        }
    }

    void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            // Raycast to detect AR planes
            if (raycastManager.Raycast(touch.position, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = raycastHits[0].pose;
                Vector3 positionWithOffset = hitPose.position + Vector3.up * heightOffset;

                // Correct the rotation to ensure the portal faces the user
                Quaternion rotation = hitPose.rotation * Quaternion.Euler(0, 180f, 0); // Flip 180° on Y-axis

                // Spawn the portal
                SpawnPortal(positionWithOffset, rotation);
            }
        }
    }

    void SpawnPortal(Vector3 position, Quaternion rotation)
    {
        // Spawn the portal prefab at the correct position and rotation
        spawnedPortal = Instantiate(prefabPortal, position, rotation);

        // Mark portal as spawned to prevent multiple spawns
        hasSpawned = true;

        // Disable plane detection after spawning
        TogglePlaneDetection();

        // Assign MainCamera to the PortalManager
        PortalManager portalManager = spawnedPortal.GetComponentInChildren<PortalManager>();
        if (portalManager != null)
        {
            portalManager.MainCamera = Camera.main.gameObject;
        }
        else
        {
            Debug.LogError("PortalManager component not found in the spawned portal!");
        }
    }

    void TogglePlaneDetection()
    {
        // Toggle plane detection on/off
        planesEnabled = !planesEnabled;
        planeManager.enabled = planesEnabled;

        // Enable/disable all tracked planes
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(planesEnabled);
        }
    }
}