using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    
    
    [FoldoutGroup("Ground Properties")][SerializeField] private float walkSpeed=150,crouchSpeed = 85;

    [FoldoutGroup("Max Move Speed Properties")] [SerializeField]private float maxControllableSpeed = 6.5f;
    public float MaxControllableSpeed => maxControllableSpeed;

    [FoldoutGroup("References")] [SerializeField]
    private Transform orientation;
    
    [FoldoutGroup("References")]
    [SerializeField]private Rigidbody rb;

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        if (CanMove()) MovePlayer();
    }

    private bool CanMove() => !PlayerWallClimb.WallClimbing &&
                              !PlayerWallClimb.WallClimbPushing &&
                              !PlayerDive.Diving &&
                              !SleighItem.Sleighing &&
                              !PlayerSliding.Sliding;

    public float GetMoveSpeed() => PlayerCrouch.Crouching ? crouchSpeed : walkSpeed;
    private void MovePlayer()
    {
        Vector3 hVel = new (rb.velocity.x,0,rb.velocity.z);
        Vector3 dir = PlayerMovementCalculator.GetFullMovementDir(PlayerMovementCalculator.GetDir(orientation), hVel, maxControllableSpeed);
        Debug.DrawLine(transform.position,transform.position + dir,Color.red);
        rb.AddForce(dir * GetMoveSpeed());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,maxControllableSpeed);
    }
}