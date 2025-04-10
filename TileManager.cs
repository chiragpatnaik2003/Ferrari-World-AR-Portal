using UnityEngine;
using UnityEngine.UI;

public class InstantTileSpawner : MonoBehaviour
{
    public GameObject tilePrefab; // The tile prefab to spawn
    public float tileLength = 20f; // Length of the tile along the Z-axis
    public float movementSpeed = 5f; // Speed at which the tile moves

    private Transform player; // Reference to the player (AR camera)
    private float spawnZ; // Z-position where the next tile will spawn
    private bool isMoving = false; // Track if the tile is moving

    void Start()
    {
        // Find the player (AR camera or main camera)
        player = Camera.main.transform;

        // Set the initial spawn position
        spawnZ = transform.position.z;

        // Ensure the tile is idle at the beginning
        isMoving = false;

        // Find the Start and Stop buttons by tag
        GameObject startButtonObj = GameObject.FindWithTag("StartButton");
        GameObject stopButtonObj = GameObject.FindWithTag("StopButton");

        // Set up button listeners
        if (startButtonObj != null)
        {
            Button startButton = startButtonObj.GetComponent<Button>();
            if (startButton != null)
            {
                startButton.onClick.AddListener(StartMovement);
            }
            else
            {
                Debug.LogError("StartButton GameObject does not have a Button component.");
            }
        }
        else
        {
            Debug.LogError("StartButton GameObject not found. Ensure the button is tagged 'StartButton'.");
        }

        if (stopButtonObj != null)
        {
            Button stopButton = stopButtonObj.GetComponent<Button>();
            if (stopButton != null)
            {
                stopButton.onClick.AddListener(StopMovement);
            }
            else
            {
                Debug.LogError("StopButton GameObject does not have a Button component.");
            }
        }
        else
        {
            Debug.LogError("StopButton GameObject not found. Ensure the button is tagged 'StopButton'.");
        }

        // Spawn the initial tile
        SpawnNextTile();
    }

    void Update()
    {
        if (isMoving)
        {
            // Move the tile backward to simulate the player moving forward
            transform.Translate(Vector3.back * movementSpeed * Time.deltaTime);

            // Check if the tile needs to be recycled
            if (player.position.z > (spawnZ + tileLength))
            {
                RecycleTile();
            }
        }
    }

    void RecycleTile()
    {
        // Move the tile ahead of the player
        spawnZ += tileLength * 2; // Move it far enough ahead to create an endless loop
        transform.position = new Vector3(transform.position.x, transform.position.y, spawnZ);

        // Spawn the next tile immediately
        SpawnNextTile();
    }

    void SpawnNextTile()
    {
        // Instantiate a new tile at the spawn position
        GameObject newTile = Instantiate(tilePrefab, new Vector3(transform.position.x, transform.position.y, spawnZ + tileLength), Quaternion.identity);
        newTile.GetComponent<InstantTileSpawner>().enabled = true; // Enable the script on the new tile
    }

    void StartMovement()
    {
        // Start the tile movement
        isMoving = true;
        Debug.Log("Tile movement started.");
    }

    void StopMovement()
    {
        // Stop the tile movement
        isMoving = false;
        Debug.Log("Tile movement stopped.");
    }
}