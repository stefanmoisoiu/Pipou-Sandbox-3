using Unity.Netcode;
using UnityEngine;

public class PGrounded : NetworkBehaviour
{
    [SerializeField] private float groundCheckRayLength;
    [SerializeField] private float maxAngle = 45;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform feetPos;
    
    
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
            transform.position + col.center - feetPos.up * col.height/2 + feetPos.up * 0.1f,
            -transform.up,
            out RaycastHit groundHit,
            groundCheckRayLength, 
            whatIsGround);
<<<<<<< Updated upstream:Assets/Player/Scripts/Movement/Core/PGrounded.cs
        IsOnControllableSlope = PMovementCalculator.CanControlMovement(groundHit.normal, maxAngle);
        
        if (IsGrounded) GroundHitRaycast = groundHit;
        rb.useGravity = !(IsGrounded &&
                          IsOnControllableSlope &&
                          PJump.CanJump &&
                          !SItem.Sleighing &&
                          !PRagdoll.Ragdolling);
=======
        Debug.Log(IsGrounded);
        IsOnControllableSlope = PlayerMovementCalculator.CanControlMovement(groundHit.normal, maxAngle);
        
        if (IsGrounded) GroundHitRaycast = groundHit;
        rb.useGravity = !(ResinMovement.OnResin ||
                          (IsGrounded &&
                           IsOnControllableSlope &&
                           PlayerJump.CanJump &&
                           !SleighItem.Sleighing &&
                           !PlayerRagdoll.Ragdolling));
>>>>>>> Stashed changes:Assets/Player/Scripts/Movement/Core/PlayerGrounded.cs
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPos = transform.position + col.center - feetPos.up * col.height / 2 + feetPos.up * 0.1f;
        Gizmos.DrawLine(startPos,startPos + -transform.up * groundCheckRayLength);
    }
}
