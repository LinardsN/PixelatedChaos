using System.Collections;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public float health = 100; // The tree's current health
    public GameObject logsPrefab; // The prefab for the wooden logs
    public float chopDelay = 1.0f; // The delay between the tree being chopped down and the logs being spawned
    public float chopRange = 1.0f; // The range at which the player can chop down the tree
    private SpriteRenderer treeSpriteRenderer; // The tree's SpriteRenderer component
    public float transparentAlpha = 0.5f; // The alpha value to set when the tree becomes transparent
    Vector3 originalPosition;
    private float lastChopTime; // The time when the tree was last chopped

    // Use this for initialization
    void Start()
    {
        treeSpriteRenderer = GetComponent<SpriteRenderer>();
        // Store the original position of the tree
        originalPosition = transform.position;

        // Initialize the last chop time to the current time
        lastChopTime = Time.time;
    }

    // Method that chops down the tree and instantiates wooden logs
    public void ChopDown(Vector2 playerPosition)
    {
        // Calculate the distance between the player and the tree
        float distance = Vector2.Distance(playerPosition, transform.position);

        // Check if the player is within range to chop down the tree
        if (distance <= chopRange)
        {
            // Get the current time
            float currentTime = Time.time;

            // Check if the current time is more than 0.5 seconds after the last chop time
            if (currentTime - lastChopTime > chopDelay)
            {
                // Update the last chop time
                lastChopTime = currentTime;

                // Reduce the tree's health
                StartCoroutine(ApplyDelay());
            }
        }
    }

    IEnumerator ApplyDelay()
    {
        yield return new WaitForSeconds(0.3f);
        health -= 10;

        // Shake the tree
        ShakeTree();

        // Check if the tree's health has reached zero or below
        if (health <= 0)
        {
            // Start the falling coroutine
            Destroy(gameObject);
            GameObject logs = Instantiate(logsPrefab, transform.position, Quaternion.identity);
        }
    }

    public void ShakeTree()
    {
        // Store the tree's current position in a temporary variable
        Vector3 originalPosition = transform.position;

        // Shake the tree using iTween.ShakePosition()
        iTween.ShakePosition(
            gameObject,
            iTween.Hash(
                "amount",
                new Vector3(0.1f, 0.1f, 0), // The amount of shaking
                "time",
                0.5f, // The duration of the shaking
                "oncomplete",
                "ResetPosition", // The name of the function to call when the shaking is complete
                "oncompletetarget",
                gameObject // The target object for the oncomplete function
            )
        );
    }

    // Method to reset the tree's position
    void ResetPosition()
    {
        // Set the tree's position back to the original position
        transform.position = originalPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is the player's collider
        if (other.gameObject.tag == "Player")
        {
            // Set the tree's transparency
            Color treeColor = treeSpriteRenderer.color;
            treeColor.a = transparentAlpha;
            treeSpriteRenderer.color = treeColor;
        }
    }

    // Method that is called when the player exits the tree's collider
    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collider is the player's collider
        if (other.gameObject.tag == "Player")
        {
            // Set the tree's transparency back to full
            Color treeColor = treeSpriteRenderer.color;
            treeColor.a = 1.0f;
            treeSpriteRenderer.color = treeColor;
        }
    }
}
