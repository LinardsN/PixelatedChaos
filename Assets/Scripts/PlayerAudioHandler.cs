using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAudioHandler : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Vector2 lastPosition;

    // Reference to the terrain detector (optional)
    [SerializeField] private TerrainDetector terrainDetector;

    // Movement threshold to determine if the player is actually moving (units per second)
    [SerializeField] private float movementThreshold = 0.05f;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

        // Store initial position
        lastPosition = transform.position;

        // If terrain detector is not assigned, try to find it on this GameObject
        if (terrainDetector == null)
        {
            terrainDetector = GetComponent<TerrainDetector>();

            // If still not found, create and add one
            if (terrainDetector == null)
            {
                terrainDetector = gameObject.AddComponent<TerrainDetector>();
            }
        }

        // Register this player with the AudioManager
        if (AudioManager.instance != null)
        {
            AudioManager.instance.RegisterPlayer(this);
        }
    }

    // Track how long the player has been moving or stationary
    private float timeInCurrentState = 0f;
    // Minimum time to consider a movement as intentional (prevents accidental triggers)
    private float minTimeForStateChange = 0.05f;
    // Flag to track if we've played a footstep for a brief movement
    private bool playedStepForBriefMovement = false;

    private void Update()
    {
        // Check if the player is moving based on position change
        Vector2 currentPosition = transform.position;
        float distance = Vector2.Distance(currentPosition, lastPosition);

        // Calculate movement speed (distance per second)
        float movementSpeed = distance / Time.deltaTime;

        // Determine if the player is moving based on speed
        bool movingNow = movementSpeed > movementThreshold;

        if (movingNow == isMoving)
        {
            // Player is continuing in the same state (moving or stationary)
            timeInCurrentState += Time.deltaTime;
        }
        else
        {
            // Player's movement state has changed
            timeInCurrentState = 0f;

            // For brief movements (tapping a key), we want to play exactly one footstep
            if (movingNow)
            {
                playedStepForBriefMovement = false;
            }
        }

        // Handle brief movements (tapping a key once)
        if (movingNow && !isMoving && !playedStepForBriefMovement && timeInCurrentState < minTimeForStateChange)
        {
            // This is a brief movement, play a single footstep
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySingleFootstep();
                playedStepForBriefMovement = true;
                // Debug.Log("Playing single footstep for brief movement");
            }
        }

        // Only update the continuous movement state after a short delay
        // This prevents rapid toggling and ensures intentional movement
        if (timeInCurrentState >= minTimeForStateChange && movingNow != isMoving)
        {
            isMoving = movingNow;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.SetPlayerMoving(isMoving);
                // Debug.Log("Player movement state changed: " + (isMoving ? "Moving" : "Stopped"));
            }
        }

        // Update last position
        lastPosition = currentPosition;
    }

    // Public method to get the current movement state
    public bool IsMoving()
    {
        return isMoving;
    }

    // Public method to get the current terrain type
    public AudioManager.TerrainType GetCurrentTerrainType()
    {
        if (terrainDetector != null)
        {
            return terrainDetector.GetCurrentTerrainType();
        }

        return AudioManager.TerrainType.Grass; // Default to grass
    }
}
