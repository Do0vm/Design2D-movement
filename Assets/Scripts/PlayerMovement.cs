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

        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            isJumping = true;
        }
    }

    private void FixedUpdate()
    {
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer) || Physics2D.OverlapCircle(propCheck.position, propRadius, propLayer);

        
        AdjustGravity();

        
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
        if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
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



    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
