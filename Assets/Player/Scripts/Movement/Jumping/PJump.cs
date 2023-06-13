using System;
using Unity.Netcode;
using UnityEngine;

public class PJump : NetworkBehaviour
{
    [SerializeField] private float jumpForce= 5, jumpCooldown = 0.2f;
    

    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerCrouchManager crouchManager;
    public static bool Jumping { get; private set; }
    public static bool CanJump { get; private set; }

    public static Action onJump;
    private void Start()
    {
        CanJump = true;
        if (IsOwner) InputManager.onJump += Jump;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (Jumping && PGrounded.IsGrounded && CanJump)
        {
            Jumping = false;
            if(InputManager.PressingCrouch) crouchManager.StartCrouchAction();
            onJump?.Invoke();
        }
    }

    private bool CanStartJump() => CanJump &&
                                   PGrounded.IsGrounded &&
                                   PGrounded.IsOnControllableSlope &&
                                   !SItem.Sleighing &&
                                   !PRagdoll.Ragdolling;
    private void Jump()
    {
        if (!CanStartJump()) return;
        CanJump = false;
        Invoke(nameof(ResetJumpCooldown),jumpCooldown);
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        Jumping = true;
        onJump?.Invoke();
    }

    public static void SetJumping(bool value)
    {
        Jumping = value;
    }
    private void ResetJumpCooldown() => CanJump = true;
}
