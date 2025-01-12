using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f; // Maximum movement speed
    public float acceleration = 15f; // How quickly the player accelerates
    public float deceleration = 10f; // How quickly the player slows down when not moving
    public float airControlFactor = 0.5f; // Reduced control in the air

    [Header("Jump Settings")]
    public float jumpForce = 15f; // Force of the jump
    public float lowJumpGravity = 2f; // Gravity multiplier for ascending jumps
    public float fallGravity = 4f; // Gravity multiplier for falling

    [Header("Ground Check")]
    public Transform groundCheck; // Point to check for ground
    public float groundRadius = 0.2f; // Radius of ground check circle
    public LayerMask groundLayer; // Layers considered as ground

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private float currentVelocityX;

    public Vector2 PlayerVelocity => rb != null ? rb.velocity : Vector2.zero;

    public bool IsGrounded()
    {
        return isGrounded;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Input handling for horizontal movement
        if (Input.GetKey(KeyCode.A))
        {
            MoveHorizontally(-1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            MoveHorizontally(1);
        }
        else
        {
            StopHorizontalMovement();
        }

        // Jump handling
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Apply gravity adjustments for better jump feel
        AdjustGravity();

        // Apply jump force
        if (isJumping)
        {
            Jump();
        }
    }

    private void MoveHorizontally(int direction)
    {
        float targetSpeed = direction * moveSpeed;
        float control = isGrounded ? 1f : airControlFactor;

        // Smoothly adjust velocity towards target speed
        currentVelocityX = Mathf.MoveTowards(rb.velocity.x, targetSpeed, acceleration * control * Time.fixedDeltaTime);

        rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);
    }



    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = false;
    }

    private void AdjustGravity()
    {
        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) // Low jump
        {
            rb.gravityScale = lowJumpGravity;
        }
        else if (rb.velocity.y < 0) // Falling
        {
            rb.gravityScale = fallGravity;
        }
        else // Default gravity
        {
            rb.gravityScale = 1f;
        }
    }

    private void StopHorizontalMovement()
    {
        if (isGrounded)
        {
            // Gradually reduce velocity when no input is detected and grounded
            currentVelocityX = Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            // In the air, keep the current velocity for inertia
            currentVelocityX = rb.velocity.x;
        }

        rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);
    }



    private void OnDrawGizmos()
    {
        // Visualize ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
