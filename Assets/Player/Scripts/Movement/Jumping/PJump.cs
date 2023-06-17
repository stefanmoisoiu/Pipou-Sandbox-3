using System;
using Unity.Netcode;
using UnityEngine;

public class PJump : NetworkBehaviour
{
    [SerializeField] private InputBind inputBind;
    [SerializeField] private ActionConditions startJumpConditions,performJumpConditions;
    
    [SerializeField] private float jumpForce= 5, jumpCooldown = 0.2f;
    
    [SerializeField] private Rigidbody rb;
    public static bool Jumping { get; private set; }
    public static bool CanJump { get; private set; }

    public static Action onJump,onCrouchLand;
    private void Start()
    {
        if (!IsOwner) return;
        CanJump = true;
        inputBind.Bind();
        BouncePlayer.onBounce += delegate { Jumping = true; };
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!performJumpConditions.ConditionsMet()) StopJump();
    }

    // private bool CanStartJump() => CanJump &&
    //                                PGrounded.IsGrounded &&
    //                                PGrounded.IsOnControllableSlope &&
    //                                !SItem.Sleighing &&
    //                                !PRagdoll.Ragdolling;
    public void Jump()
    {
        if (!startJumpConditions.ConditionsMet()) return;
        CanJump = false;
        Invoke(nameof(ResetJumpCooldown),jumpCooldown);
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        Jumping = true;
        onJump?.Invoke();
    }

    private void StopJump()
    {
        if (!Jumping || CanJump == false) return;
        Jumping = false;
        if(InputManager.PressingCrouch) onCrouchLand?.Invoke();
    }
    private void ResetJumpCooldown() => CanJump = true;
}
