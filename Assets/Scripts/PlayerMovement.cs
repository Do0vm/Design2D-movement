using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 10f;
    public float airControlFactor = 0.5f;

    [Header("Jump Settings")]
    public float jumpForce = 15f;
    public float lowJumpGravity = 2f;
    public float fallGravity = 4f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    public Transform propCheck;
    public float propRadius = 0.2f;
    public LayerMask propLayer;

    [Header("Audio Settings")]
    public AudioClip jumpSound;         // Audio for jump
    public AudioClip rocketSound;       // Audio for rocket sound
    private AudioSource audioSource;    // AudioSource to play the clips

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool isJumping = false;
    private float currentVelocityX;

    public Vector2 PlayerVelocity => rb != null ? rb.velocity : Vector2.zero;

    public bool IsGrounded()
    {
        return isGrounded;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Attach AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Horizontal Movement (A, D and Gamepad Left Stick)
        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            horizontalInput = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            horizontalInput = 1;
        }
        else
        {
            horizontalInput = Input.GetAxis("Horizontal"); // For gamepad left stick
        }

        // Move horizontally based on input
        if (horizontalInput != 0f)
        {
            MoveHorizontally((int)horizontalInput);
        }
        else
        {
            StopHorizontalMovement();
        }

        // Jump Action (JoystickButton0 instead of Space)
        if (Input.GetKeyDown(KeyCode.JoystickButton0)) // JoystickButton0 is the 'A' button on a controller
        {
            if (isGrounded)
            {
                isJumping = true;
                PlayAudio(jumpSound); // Play jump sound
            }
        }

        // Play rocket sound while holding joystick button and ascending
        if (Input.GetKey(KeyCode.JoystickButton0) && rb.velocity.y > 0)
        {
            if (!audioSource.isPlaying || audioSource.clip != rocketSound)
            {
                audioSource.clip = rocketSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying && audioSource.clip == rocketSound)
        {
            audioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        // Ground Check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer) ||
                     Physics2D.OverlapCircle(propCheck.position, propRadius, propLayer);


        Debug.Log($"FixedUpdate - IsGrounded: {isGrounded}, GroundCheck Position: {groundCheck.position}, " +
             $"PropCheck Position: {propCheck.position}, Velocity: {rb.velocity}, GravityScale: {rb.gravityScale}");

        // Adjust Gravity
        AdjustGravity();

        // Execute Jump
        if (isJumping)
        {
            Jump();
        }
    }

    private void MoveHorizontally(int direction)
    {
        float targetSpeed = direction * moveSpeed;
        float control = isGrounded ? 1f : airControlFactor;

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
        if (rb.velocity.y > 0 && (!Input.GetKey(KeyCode.JoystickButton0)))
        {
            rb.gravityScale = lowJumpGravity;
        }
        else if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallGravity;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void StopHorizontalMovement()
    {
        if (isGrounded)
        {
            currentVelocityX = Mathf.MoveTowards(rb.velocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocityX = rb.velocity.x;
        }
        rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
