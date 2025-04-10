using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class CarAnimationController : MonoBehaviour
{
    [Header("Animation Control")]
    public string animationTrigger = "EngineTrigger"; // Customizable trigger name
    public string resetTrigger = "ResetEngine"; // Optional reset trigger

    [Header("Sound")]
    public AudioClip engineSound;
    [Range(0, 1)] public float soundVolume = 0.8f;

    [Header("Button Setup")]
    public string buttonTag = "CarButton";

    private Animator carAnimator;
    private AudioSource audioSource;
    private Button controlButton;
    private bool isEngineOn;

    void Start()
    {
        // Get components
        carAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Configure audio
        audioSource.playOnAwake = false;
        audioSource.clip = engineSound;
        audioSource.volume = soundVolume;

        // Setup button
        GameObject buttonObj = GameObject.FindGameObjectWithTag(buttonTag);
        if (buttonObj != null)
        {
            controlButton = buttonObj.GetComponent<Button>();
            if (controlButton != null)
            {
                controlButton.onClick.AddListener(ToggleCarAnimation);
            }
            else Debug.LogError("No Button component found on tagged object");
        }
        else Debug.LogError($"No object with tag '{buttonTag}' found");
    }

    public void ToggleCarAnimation()
    {
        isEngineOn = !isEngineOn;

        if (isEngineOn)
        {
            // Start animation and sound
            carAnimator.SetTrigger(animationTrigger);
            audioSource.Play();
            Debug.Log($"Triggered animation: {animationTrigger}");
        }
        else
        {
            // Reset animation if using separate trigger
            if (!string.IsNullOrEmpty(resetTrigger))
                carAnimator.SetTrigger(resetTrigger);

            audioSource.Stop();
            Debug.Log("Stopped car animation");
        }
    }
}