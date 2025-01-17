using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab; // The projectile to be instantiated
    public float launchSpeed = 20f;     // Speed at which the projectile is launched
    public float lifetime = 3f;         // Time before the projectile is destroyed

    [Header("Spawn Settings")]
    public Transform spawnPoint;        // The point where the projectile is spawned
    public float spawnRate = 0.1f;      // Time interval between each spawn (higher rate = lower time)

    private float nextSpawnTime = 0f;   // Tracks the next allowed spawn time

    [Header("Player Settings")]
    public Rigidbody2D playerRigidbody; // The Rigidbody2D of the player
    public float fallingThreshold = -0.1f; // Velocity below which the player is considered falling

    [Header("Ground Check")]
    public Transform groundCheck;       // Ground check position
    public float groundRadius = 0.2f;   // Radius of the ground check
    public LayerMask groundLayers;      // Layers considered as ground (e.g., floor, prop)

    private bool isSpaceHeld = false;  // Track if space bar is being held down

    private void Update()
    {
        // Detect when the space bar is being held down
        if (Input.GetKey(KeyCode.JoystickButton0))
        {
            isSpaceHeld = true;
        }
        else
        {
            isSpaceHeld = false;
        }

        // Only spawn projectiles if the space bar is held and the player is grounded or not falling
        if (isSpaceHeld && Time.time >= nextSpawnTime && (IsGrounded() || !IsPlayerFalling()))
        {
            ShootProjectile();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    private void ShootProjectile()
    {
        // Check if a spawn point and prefab are assigned
        if (projectilePrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("ProjectilePrefab or SpawnPoint is not assigned.");
            return;
        }

        // Instantiate the projectile at the spawn point's position
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        // Apply downward velocity to the projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.down * launchSpeed;
        }

        // Destroy the projectile after the specified lifetime
        Destroy(projectile, lifetime);
    }

    private bool IsPlayerFalling()
    {
        // Check if the player's vertical velocity is below the falling threshold
        return playerRigidbody != null && playerRigidbody.velocity.y < fallingThreshold;
    }

    private bool IsGrounded()
    {
        // Check if the player is touching the ground layers
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayers);
    }

    private void OnDrawGizmos()
    {
        // Visualize the ground check radius in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}
