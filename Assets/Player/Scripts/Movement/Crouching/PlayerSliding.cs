using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PlayerSliding : NetworkBehaviour,IPlayerCrouchAction
{
    [FoldoutGroup("Properties")] [SerializeField]
    private float slideStopSpeedThreshold = 3,startSlideForce = 20;
    [FoldoutGroup("Properties")] [SerializeField]
    private PlayerSlideCalculator.SlideDampingProperties slideDampingProperties;

    
    
    [FoldoutGroup("Slide FOV")] [SerializeField]
    private PlayerCamFOV.ConstantFOV slideFOV;

    [FoldoutGroup("Slide FOV")] [SerializeField]
    private float slideAddedFOV = 10;
    [FoldoutGroup("Slide FOV")] [SerializeField]
    private float slideAddedFOVPerVelocity = .5f;

    
    
    
    [FoldoutGroup("References")] [SerializeField]
    private Transform orientation;

    [SerializeField]private Rigidbody rb;
    [SerializeField]private PlayerHeight playerHeight;
    public static bool Sliding { get; private set; }

    private void Start()
    {
        if (!IsOwner) return;
        PlayerCamFOV.AddFOV(slideFOV);
        ConveyorBelt.onStartUsing += StopAction;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (!Sliding)
        {
            slideFOV.SetNewAmount(0);
            return;
        }

        Slide();
        
        if(!CanPerformAction()) StopAction();
        
        slideFOV.SetNewAmount(slideAddedFOV + rb.velocity.magnitude * slideAddedFOVPerVelocity);
    }

    private void Slide()
    {
        PlayerSlideCalculator.ApplySlideDamping(rb.velocity,slideDampingProperties);
    }
    public bool CanStartAction() => PlayerGrounded.IsGrounded &&
                                    !Sliding &&
                                    Vector2.Dot(InputManager.MoveInput,Vector2.up) > 0.2f &&
                                    !SleighItem.Sleighing;

    private bool CanPerformAction() => !SleighItem.Sleighing && 
                                       PlayerJump.CanJump &&
                                       !PlayerJump.Jumping &&
                                       rb.velocity.magnitude >= slideStopSpeedThreshold;
    public void StartAction()
    {
        Sliding = true;
        playerHeight.SetHeight(PlayerHeight.HeightType.Crouch);
        slideFOV.SetNewAmount(slideAddedFOV);

        if (PlayerGrounded.IsOnControllableSlope)
        {
            Vector3 force = orientation.forward * startSlideForce;
            PlayerMovementCalculator.GetGroundProjectedVector(force);
            rb.AddForce(force,ForceMode.Impulse);
        }

        Slide();
    }

    public void StopAction()
    {
        if (!Sliding) return;
        
        Sliding = false;
        PlayerDamping.ResetDamping();
        if(!SleighItem.Sleighing) playerHeight.SetHeight(PlayerHeight.HeightType.Normal);
        slideFOV.SetNewAmount(0);
    }
}