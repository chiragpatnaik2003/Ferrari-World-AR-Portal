using UnityEngine;
using System.Collections; // Add this namespace for IEnumerator

[RequireComponent(typeof(Collider))]
public class PortalExitHandler : MonoBehaviour
{
    [Header("Materials")]
    public Material portalMaterial;
    public Material normalMaterial;

    [Header("Transition Settings")]
    public float transitionDuration = 1.0f;
    public string stencilProperty = "_StencilComp";

    private Renderer carRenderer;
    private bool isInPortal = true;
    private float transitionProgress = 0f;

    void Start()
    {
        carRenderer = GetComponent<Renderer>();
        carRenderer.materials = new Material[] { portalMaterial };
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PortalExit"))
        {
            StartCoroutine(TransitionMaterials());
        }
    }

    IEnumerator TransitionMaterials()
    {
        while (transitionProgress < 1f)
        {
            transitionProgress += Time.deltaTime / transitionDuration;

            // Blend between stencil and normal rendering
            if (isInPortal)
            {
                portalMaterial.SetFloat(stencilProperty,
                    Mathf.Lerp(1, 0, transitionProgress)); // From stencil to normal
            }

            yield return null;
        }

        // Final material switch
        carRenderer.materials = new Material[] { normalMaterial };
        isInPortal = false;
    }
}