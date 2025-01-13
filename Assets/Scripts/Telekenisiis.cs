using System.Collections.Generic;
using UnityEngine;

public class TelekinesisController : MonoBehaviour
{
    [Header("Telekinesis Settings")]
    public float radius = 5f; 
    public float liftForce = 10f; 
    public float hoverHeight = 2f;
    public float hoverSmoothing = 5f; 
    public float attractSpeed = 5f; 
    public LayerMask propLayer; 

    private GameObject heldProp; 
    private Rigidbody heldRigidbody; 
    private bool isHolding;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (isHolding) 
            {
                DropProp();
            }
            else
            {
                TryGrabProp();
            }
        }

        if (isHolding && heldProp != null)
        {
            MoveHeldProp();
        }
    }

    private void TryGrabProp()
    {
        Collider[] propsInRadius = Physics.OverlapSphere(transform.position, radius, propLayer);
        foreach (var collider in propsInRadius)
        {
            if (collider.CompareTag("Prop"))
            {
                Rigidbody propRigidbody = collider.GetComponent<Rigidbody>();
                if (propRigidbody != null)
                {
                    heldProp = collider.gameObject;
                    heldRigidbody = propRigidbody;

                    heldRigidbody.useGravity = false;
                    heldRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                    isHolding = true;
                    break;
                }
            }
        }
    }

    private void DropProp()
    {
        if (heldProp != null && heldRigidbody != null)
        {
            heldRigidbody.useGravity = true;
            heldRigidbody.constraints = RigidbodyConstraints.None;

            heldProp.transform.parent = null;
            heldProp = null;
            heldRigidbody = null;
            isHolding = false;
        }
    }

    private void MoveHeldProp()
    {
        Vector3 targetPosition = transform.position + transform.forward * 2f + Vector3.up * hoverHeight;
        Vector3 direction = targetPosition - heldProp.transform.position;

        heldRigidbody.velocity = direction * attractSpeed;

        heldProp.transform.parent = transform;
    }

    private void FixedUpdate()
    {
        if (!isHolding)
        {
            Collider[] propsInRadius = Physics.OverlapSphere(transform.position, radius, propLayer);
            foreach (var collider in propsInRadius)
            {
                if (collider.CompareTag("Prop"))
                {
                    Rigidbody propRigidbody = collider.GetComponent<Rigidbody>();
                    if (propRigidbody != null)
                    {
                        Vector3 targetHoverPosition = collider.transform.position + Vector3.up * hoverHeight;
                        Vector3 forceDirection = targetHoverPosition - collider.transform.position;

                        propRigidbody.AddForce(Vector3.up * liftForce + forceDirection * hoverSmoothing, ForceMode.Acceleration);
                    }
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
