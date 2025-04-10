using UnityEngine;
using UnityEngine.UI;

public class MegaConfettiController : MonoBehaviour
{
    [Header("ASSIGN IN INSPECTOR")]
    public ParticleSystem[] confettiSystems = new ParticleSystem[11]; // Array for 11 systems
    public AudioClip confettiSound; // Assign your sound clip
    public string buttonTag = "ConfettiButton";

    private Button confettiButton;
    private bool isConfettiActive;
    private AudioSource audioSource;

    void Start()
    {
        // Set up audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = confettiSound;
        audioSource.loop = false;

        // 1. Find button by tag
        GameObject buttonObj = GameObject.FindGameObjectWithTag(buttonTag);
        if (buttonObj == null)
        {
            Debug.LogError("No object with tag '" + buttonTag + "' found!");
            return;
        }

        // 2. Get Button component
        confettiButton = buttonObj.GetComponent<Button>();
        if (confettiButton == null)
        {
            Debug.LogError("No Button component found on tagged object!");
            return;
        }

        // 3. Initialize all particle systems
        foreach (ParticleSystem ps in confettiSystems)
        {
            if (ps != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = ps.main;
                main.loop = true; // Ensure looping is enabled
            }
            else
            {
                Debug.LogWarning("Empty slot in confettiSystems array!");
            }
        }

        // 4. Setup button listener
        confettiButton.onClick.AddListener(ToggleAllConfetti);
    }

    public void ToggleAllConfetti()
    {
        isConfettiActive = !isConfettiActive;

        foreach (ParticleSystem ps in confettiSystems)
        {
            if (ps == null) continue;

            if (isConfettiActive)
            {
                ps.Play();
                Debug.Log("Playing: " + ps.name);
            }
            else
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                Debug.Log("Stopping: " + ps.name);
            }
        }

        // Play sound only when activating confetti (not when stopping)
        if (isConfettiActive && confettiSound != null)
        {
            audioSource.Play();
        }
    }
}