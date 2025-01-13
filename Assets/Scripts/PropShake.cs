using UnityEngine;

public class PropShake : MonoBehaviour
{
    public float forceMultiplier = 5f; // Controls the strength of the force applied
    public LayerMask groundLayer; // Layer that defines the ground
    public float groundCheckRadius = 0.1f; // Radius for ground check
    public Transform groundCheckPoint; // Point to check if the prop is grounded

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Awake()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError($"Rigidbody2D not found on {gameObject.name}. PropShake requires a Rigidbody2D to work.");
        }

        if (groundCheckPoint == null)
        {
            Debug.LogError($"GroundCheckPoint is not assigned on {gameObject.name}. Please assign a transform for ground checking.");
        }
    }

    private void Update()
    {
        CheckIfGrounded();
    }

    private void OnEnable()
    {
        // Subscribe to the OnScreenShake event
        CameraScreenShake.OnScreenShake += ApplyUpwardForce;
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnScreenShake event
        CameraScreenShake.OnScreenShake -= ApplyUpwardForce;
    }

    private void CheckIfGrounded()
    {
        if (groundCheckPoint != null)
        {
            // Check if the prop is touching the ground
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }
    }

    private void ApplyUpwardForce(float duration, float magnitude)
    {
        if (rb != null && isGrounded)
        {
            // Apply an upward force, scaled by the magnitude of the shake
            Vector2 upwardForce = Vector2.up * magnitude * forceMultiplier;
            rb.AddForce(upwardForce, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the ground check radius in the Scene view
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
