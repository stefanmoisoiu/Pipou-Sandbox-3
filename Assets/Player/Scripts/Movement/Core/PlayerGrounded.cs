using Unity.Netcode;
using UnityEngine;

public class PlayerGrounded : NetworkBehaviour
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
            transform.position + col.center - Vector3.up * col.height/2 + Vector3.up * 0.1f,
            -transform.up,
            out RaycastHit groundHit,
            groundCheckRayLength, 
            whatIsGround);
        IsOnControllableSlope = PlayerMovementCalculator.CanControlMovement(groundHit.normal, maxAngle);
        
        if (IsGrounded) GroundHitRaycast = groundHit;
        rb.useGravity = !(IsGrounded &&
                          !IsOnControllableSlope &&
                          PlayerJump.CanJump &&
                          !SleighItem.Sleighing);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPos = transform.position + col.center - Vector3.up * col.height / 2 + Vector3.up * 0.1f;
        Gizmos.DrawLine(startPos,startPos + -transform.up * groundCheckRayLength);
    }
}
