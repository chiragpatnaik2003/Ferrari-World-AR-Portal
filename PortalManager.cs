using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalManager : MonoBehaviour
{
    public GameObject station; // Reference to the environment/station (assigned in prefab)
    public GameObject portalPlane; // Reference to the portal plane (assigned in prefab)

    public GameObject MainCamera; // Reference to the main camera (assigned by PortalSpawner)

    private List<Material> stationMaterials = new List<Material>(); // List to store materials of the station

    void Start()
    {
        if (MainCamera == null)
        {
            Debug.LogError("Main Camera not assigned! Ensure the PortalSpawner assigns it.");
            return;
        }

        // Ensure the portal plane has a collider and rigidbody
        Collider portalCollider = portalPlane.GetComponent<Collider>();
        if (portalCollider == null)
        {
            portalCollider = portalPlane.AddComponent<BoxCollider>(); // Add a collider if not already present
        }
        portalCollider.isTrigger = true; // Enable Is Trigger

        Rigidbody portalRigidbody = portalPlane.GetComponent<Rigidbody>();
        if (portalRigidbody == null)
        {
            portalRigidbody = portalPlane.AddComponent<Rigidbody>();
        }
        portalRigidbody.isKinematic = true; // Set the rigidbody to kinematic
        portalRigidbody.useGravity = false; // Disable gravity

        // Ensure the main camera has a collider and rigidbody
        Collider cameraCollider = MainCamera.GetComponent<Collider>();
        if (cameraCollider == null)
        {
            cameraCollider = MainCamera.AddComponent<SphereCollider>(); // Add a collider if not already present
        }
        cameraCollider.isTrigger = true; // Enable Is Trigger

        Rigidbody cameraRigidbody = MainCamera.GetComponent<Rigidbody>();
        if (cameraRigidbody == null)
        {
            cameraRigidbody = MainCamera.AddComponent<Rigidbody>();
        }
        cameraRigidbody.isKinematic = true; // Set the rigidbody to kinematic
        cameraRigidbody.useGravity = false; // Disable gravity

        // Get all child renderers of the station
        Renderer[] stationRenderers = station.GetComponentsInChildren<Renderer>();

        // Collect all materials from the station
        foreach (Renderer renderer in stationRenderers)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat.HasProperty("_StencilComp")) // Ensure the material supports stencil operations
                {
                    stationMaterials.Add(mat);
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        // Check if the collider is the main camera
        if (collider.gameObject == MainCamera)
        {
            Debug.Log("Camera entered the portal plane.");

            // Set the stencil comparison to "Always" to show the environment
            UpdateMaterials(stationMaterials, (int)CompareFunction.Always);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        // Check if the collider is the main camera
        if (collider.gameObject == MainCamera)
        {
            Debug.Log("Camera exited the portal plane.");

            // Reset the stencil comparison to "Equal" to hide the environment
            UpdateMaterials(stationMaterials, (int)CompareFunction.Equal);
        }
    }

    private void UpdateMaterials(List<Material> materials, int stencilValue)
    {
        // Update the stencil comparison for all materials in the list
        foreach (Material mat in materials)
        {
            mat.SetInt("_StencilComp", stencilValue);
        }
    }
}