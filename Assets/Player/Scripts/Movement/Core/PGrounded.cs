using Unity.Netcode;
using UnityEngine;

public class PGrounded : NetworkBehaviour
{
    [SerializeField] private float groundCheckRayLength;
    [SerializeField] private float maxAngle = 45;
    [SerializeField] private LayerMask whatIsGround;
    
    public static bool IsGrounded { get; private set; }
    public static bool IsOnControllableSlope { get; private set; }
    public static RaycastHit GroundHitRaycast { get; private set; }

    [SerializeField]private Rigidbody rb;
    [SerializeField]private CapsuleCollider col;

    private void Update()
    {
        if (!IsOwner) return;
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        IsGrounded = Physics.Raycast(
            transform.position + col.center * transform.root.localScale.x,
            -transform.up,
            out RaycastHit groundHit,
            groundCheckRayLength * Mathf.Min(transform.root.localScale.x,1) + col.height / 2  * transform.root.localScale.x, 
            whatIsGround);
        IsOnControllableSlope = PMovementCalculator.CanControlMovement(groundHit.normal, maxAngle);
        
        if (IsGrounded) GroundHitRaycast = groundHit;
        rb.useGravity = !(IsGrounded &&
                          IsOnControllableSlope &&
                          PJump.CanJump &&
                          !SItem.Sleighing &&
                          !PRagdoll.Ragdolling);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPos = transform.position + col.center * transform.root.localScale.x;
        Gizmos.DrawLine(startPos,startPos -transform.up * col.height / 2 * transform.root.localScale.x -transform.up * groundCheckRayLength * Mathf.Min(transform.root.localScale.x,1));
    }
}
