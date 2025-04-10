using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering;

[RequireComponent(typeof(Renderer), typeof(Collider), typeof(Animator))]
public class PortalVideoPlayer : MonoBehaviour
{
    public VideoClip portalVideo;
    public RenderTexture targetTexture;
    public float triggerDistance = 2f;
    public string animationBoolName = "IsPlaying"; // Using bool instead of trigger

    private VideoPlayer videoPlayer;
    private Material videoMaterial;
    private Animator animator;
    private GameObject mainCamera;
    private bool isNear = false; // Tracks whether the player is near

    void Start()
    {
        mainCamera = Camera.main.gameObject;
        animator = GetComponent<Animator>();

        // Set up video player
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.clip = portalVideo;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = targetTexture;

        // Set up material
        videoMaterial = new Material(Shader.Find("Custom/PortalVideoShader"));
        GetComponent<Renderer>().material = videoMaterial;
        videoMaterial.mainTexture = targetTexture;
        videoMaterial.SetInt("_StencilComp", (int)CompareFunction.Equal);

        // Set up audio
        var audioSource = gameObject.AddComponent<AudioSource>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Pause video at start
        videoPlayer.Pause();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

        if (distance <= triggerDistance && !isNear)
        {
            StartVideo();
        }
        else if (distance > triggerDistance && isNear)
        {
            StopVideo();
        }
    }

    void StartVideo()
    {
        isNear = true;

        // Set animator bool to true (starts animation)
        animator.SetBool(animationBoolName, true);

        // Play video
        videoPlayer.Play();

        Debug.Log("Video started playing.");
    }

    void StopVideo()
    {
        isNear = false;

        // Set animator bool to false (transitions back to idle)
        animator.SetBool(animationBoolName, false);

        // Stop video and reset playback
        videoPlayer.Stop();

        Debug.Log("Video stopped playing.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isNear && other.gameObject == mainCamera)
        {
            StartVideo();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isNear && other.gameObject == mainCamera)
        {
            StopVideo();
        }
    }
}
