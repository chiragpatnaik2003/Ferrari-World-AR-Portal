using UnityEngine;

public class TrackController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float maxSpeed = 30f;
    public float acceleration = 5f;
    public float turnSpeed = 25f;

    [Header("References")]
    public Transform carVisual; // Your Ferrari model
    public Transform trackParent; // Parent object of your track

    private float currentSpeed;
    private float currentTurn;
    private Vector3 initialTrackPosition;

    void Start()
    {
        initialTrackPosition = trackParent.position;
    }

    void Update()
    {
        HandleMovement();
        HandleSteering();
    }

    void HandleMovement()
    {
        // Move track backward to create forward illusion
        trackParent.Translate(Vector3.back * currentSpeed * Time.deltaTime, Space.World);

        // Keep car visually centered (adjust local position if needed)
        carVisual.localPosition = Vector3.zero;
    }

    void HandleSteering()
    {
        // Rotate entire track for turning illusion
        trackParent.RotateAround(carVisual.position, Vector3.up, -currentTurn * Time.deltaTime);

        // Tilt car slightly during turns
        float tiltAmount = -currentTurn * 0.5f;
        carVisual.localRotation = Quaternion.Euler(0, 0, tiltAmount);
    }

    // UI Control Methods
    public void Accelerate() => currentSpeed = Mathf.Min(currentSpeed + acceleration, maxSpeed);
    public void Brake() => currentSpeed = Mathf.Max(0, currentSpeed - acceleration * 2f);
    public void TurnLeft() => currentTurn = turnSpeed;
    public void TurnRight() => currentTurn = -turnSpeed;
    public void Straighten() => currentTurn = 0f;

    public void ResetTrackPosition()
    {
        trackParent.position = initialTrackPosition;
        trackParent.rotation = Quaternion.identity;
    }
}