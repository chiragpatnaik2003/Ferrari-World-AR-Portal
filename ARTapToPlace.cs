using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObject : MonoBehaviour
{
    public GameObject gameObjectToInstantiate; // The portal prefab to spawn
    public ARPlaneManager planeManager; // AR Plane Manager
    public Button togglePlaneButton; // Button to toggle plane detection
    public Button rotateButton; // Button to rotate the object
    public Scrollbar sizeScrollbar; // UI Scrollbar for scaling
    public float heightOffset = 0.1f; // Height offset for spawned objects
    public float minScale = 0.1f, maxScale = 2.0f; // Scale range
    public float rotationDuration = 2.0f; // Duration of the rotation (in seconds)

    private GameObject spawnedObject; // The spawned object
    private ARRaycastManager raycastManager; // AR Raycast Manager
    private bool isPlaneDetectionActive = true; // Track if plane detection is active
    private bool hasSpawned = false; // Track if the object has been spawned
    static List<ARRaycastHit> hits = new List<ARRaycastHit>(); // List of AR raycast hits

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();

        if (togglePlaneButton != null)
        {
            togglePlaneButton.onClick.AddListener(TogglePlaneDetection);
        }

        if (sizeScrollbar != null)
        {
            sizeScrollbar.onValueChanged.AddListener(UpdateObjectScale);
        }

        if (rotateButton != null)
        {
            rotateButton.onClick.AddListener(RotateObjectGradually);
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    void Update()
    {
        // Skip if the object has already been spawned
        if (hasSpawned)
            return;

        // Skip if plane detection is inactive or no touch input is detected
        if (!isPlaneDetectionActive || !TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            Vector3 positionWithOffset = hitPose.position + Vector3.up * heightOffset;

            // Spawn the object only once
            spawnedObject = Instantiate(gameObjectToInstantiate, positionWithOffset, hitPose.rotation);

            // Rotate the portal to face the user (Fix orientation)
            spawnedObject.transform.Rotate(0, 180f, 0); // Flip Y-axis to face correctly

            UpdateObjectScale(sizeScrollbar.value); // Set initial scale

            // Mark the object as spawned
            hasSpawned = true;

            // Disable plane detection after spawning
            TogglePlaneDetection();
        }
    }

    public void TogglePlaneDetection()
    {
        isPlaneDetectionActive = !isPlaneDetectionActive;
        if (planeManager != null)
        {
            planeManager.enabled = isPlaneDetectionActive;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(isPlaneDetectionActive);
            }
        }
        Debug.Log("Plane Detection: " + (isPlaneDetectionActive ? "Enabled" : "Disabled"));
    }

    public void UpdateObjectScale(float value)
    {
        if (spawnedObject != null)
        {
            float scaleFactor = Mathf.Lerp(minScale, maxScale, value);
            spawnedObject.transform.localScale = Vector3.one * scaleFactor;
        }
    }

    public void RotateObjectGradually()
    {
        if (spawnedObject != null)
        {
            StartCoroutine(RotateObjectCoroutine(spawnedObject.transform, 180f, rotationDuration));
        }
    }

    private IEnumerator RotateObjectCoroutine(Transform objTransform, float targetAngle, float duration)
    {
        Quaternion startRotation = objTransform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, targetAngle, 0);
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            objTransform.rotation = Quaternion.Slerp(startRotation, endRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        objTransform.rotation = endRotation; // Ensure it ends exactly at the target angle
    }
}