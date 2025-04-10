using UnityEngine;
using UnityEngine.UI;

public class PanelAnimationTrigger : MonoBehaviour
{
    public Animator panelAnimator; // Reference to the Animator component
    public string animationTriggerName = "PlayAnimation"; // Animation trigger name
    public AudioSource audioSource; // Reference to AudioSource component
    private bool hasPlayedAudio = false; // Ensures audio plays only once

    private void Start()
    {
        // Find the Stop Button using its tag
        GameObject stopButton = GameObject.FindGameObjectWithTag("StopAudioButton");

        if (stopButton != null)
        {
            Button buttonComponent = stopButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(StopAudio);
            }
        }
        else
        {
            Debug.LogWarning("Stop Button with tag 'StopAudioButton' not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the AR Camera (or the player)
        if (other.CompareTag("MainCamera"))
        {
            // Trigger the animation
            if (panelAnimator != null)
            {
                panelAnimator.SetTrigger(animationTriggerName);
            }

            // Play audio ONLY ONCE, even if the player re-enters
            if (!hasPlayedAudio && audioSource != null)
            {
                audioSource.Play();
                hasPlayedAudio = true; // Prevents the audio from restarting ever again
            }
        }
    }

    private void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
