using UnityEngine;
using System;

public class CameraScreenShake : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    public Transform target;
    public Vector3 posOffset;
    public float smooth = 5f;

    [Header("Shake Settings")]
    public float shakeThreshold = -20f;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;

    public static event Action<float, float> OnScreenShake; // Event for props to listen to

    private PlayerMovement playerMovement;
    private float shakeTimer;
    private Vector3 shakeOffset;
    private bool wasGroundedLastFrame;

    private void Start()
    {
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

        wasGroundedLastFrame = false;
    }

    private void Update()
    {
        if (playerMovement != null)
        {
            bool isGrounded = playerMovement.IsGrounded();
            if (!wasGroundedLastFrame && isGrounded && playerMovement.PlayerVelocity.y < shakeThreshold)
            {
                TriggerScreenShake();
            }

            wasGroundedLastFrame = isGrounded;
        }

        if (shakeTimer > 0)
        {
            ApplyScreenShake();
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + posOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition + shakeOffset, smooth * Time.deltaTime);
    }

    private void TriggerScreenShake()
    {
        shakeTimer = shakeDuration;

        // Notify all props about the shake
        OnScreenShake?.Invoke(shakeDuration, shakeMagnitude);
    }

    private void ApplyScreenShake()
    {
        shakeOffset = UnityEngine.Random.insideUnitSphere * shakeMagnitude;
        shakeOffset.z = 0;

        shakeTimer -= Time.deltaTime;
    }
}
