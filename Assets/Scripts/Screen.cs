using UnityEngine;

public class CameraScreenShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeThreshold = -20f; // Falling velocity to trigger shake
    public float shakeDuration = 0.2f; // Duration of the screen shake
    public float shakeMagnitude = 0.1f; // Magnitude of the screen shake

    private PlayerMovement playerMovement;
    private Vector3 originalPosition;
    private float shakeTimer;
    private bool wasGroundedLastFrame;

    private void Start()
    {
        // Find and reference the PlayerMovement script
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();

            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement script not found on the player object.");
            }
        }
        else
        {
            Debug.LogError("Player object with 'Player' tag not found.");
        }

        originalPosition = transform.position;
        wasGroundedLastFrame = false;
    }

    private void Update()
    {
        if (playerMovement != null)
        {
            // Check if the player has landed and their falling velocity exceeds the threshold
            bool isGrounded = playerMovement.IsGrounded();
            if (!wasGroundedLastFrame && isGrounded && playerMovement.PlayerVelocity.y < shakeThreshold)
            {
                TriggerScreenShake();
            }

            wasGroundedLastFrame = isGrounded;
        }

        // Apply the shake effect if the timer is active
        if (shakeTimer > 0)
        {
            ApplyScreenShake();
        }
    }

    private void TriggerScreenShake()
    {
        shakeTimer = shakeDuration;
    }

    private void ApplyScreenShake()
    {
        Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
        transform.position = originalPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0);

        shakeTimer -= Time.deltaTime;

        if (shakeTimer <= 0)
        {
            transform.position = originalPosition;
        }
    }
}
