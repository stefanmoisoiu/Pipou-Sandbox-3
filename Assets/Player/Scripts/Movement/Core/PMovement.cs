using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PMovement : NetworkBehaviour
{
    [SerializeField] private ActionConditions startMovementConditions;
    
    
    [FoldoutGroup("Ground Properties")][SerializeField] private float walkSpeed=150,crouchSpeed = 85;

    [FoldoutGroup("Max Move Speed Properties")] [SerializeField]private float maxControllableSpeed = 6.5f;

    [FoldoutGroup("References")] [SerializeField]
    private Transform orientation;
    
    [FoldoutGroup("References")]
    [SerializeField]private Rigidbody rb;
    public static bool TooFast { get; private set; }
    public static bool MovingForward { get; private set; }
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        
        if (startMovementConditions.ConditionsMet()) MovePlayer();

        MovingForward = Vector2.Dot(InputManager.MoveInput, Vector2.up) > 0f;
    }

    // private bool CanMove() => !PlayerWallClimb.WallClimbing &&
    //                           !PlayerWallClimb.WallClimbPushing &&
    //                           !PlayerDive.Diving &&
    //                           !SleighItem.Sleighing &&
    //                           !PlayerSlide.Sliding &&
    //                           !PlayerRagdoll.Ragdolling &&
    //                           !DialogueManager.Talking;

    public float GetMoveSpeed() => PCrouch.Crouching ? crouchSpeed : walkSpeed;
    private void MovePlayer()
    {
        Vector3 hVel = new (rb.velocity.x,0,rb.velocity.z);
        Vector3 dir = PMovementCalculator.GetFullMovementDir(PMovementCalculator.GetDir(orientation), hVel, maxControllableSpeed,out bool tooFast);
        TooFast = tooFast;
        Debug.DrawLine(transform.position,transform.position + dir,Color.red);
        rb.AddForce(dir * GetMoveSpeed());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,maxControllableSpeed);
    }
}