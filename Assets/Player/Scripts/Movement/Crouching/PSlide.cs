﻿using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PSlide : NetworkBehaviour
{
    [FoldoutGroup("Properties")] [SerializeField] private InputBind startSlideBind,stopSlideBind;
    [FoldoutGroup("Properties")] [SerializeField] private ActionConditions startSlideConditions,performSlideConditions;
    
    [FoldoutGroup("Properties")] [SerializeField]
    private float slideStopSpeedThreshold = 3,startSlideForce = 20;
    [FoldoutGroup("Properties")] [SerializeField]
    private PlayerSlideCalculator.SlideDampingProperties slideDampingProperties;

    
    
    [FoldoutGroup("Slide FOV")] [SerializeField]
    private PCamFOV.ConstantFOV slideFOV;

    [FoldoutGroup("Slide FOV")] [SerializeField]
    private float slideAddedFOV = 10;
    [FoldoutGroup("Slide FOV")] [SerializeField]
    private float slideAddedFOVPerVelocity = .5f;

    
    
    
    [FoldoutGroup("References")] [SerializeField]
    private Transform orientation;

    [SerializeField]private Rigidbody rb;
    [SerializeField]private PHeight pHeight;
    public static bool Sliding { get; private set; }

    private void Start()
    {
        if (!IsOwner) return;
        startSlideBind.Bind();
        stopSlideBind.Bind();
        
        PDive.onStopDiveCrouch += StartSlide;
        PJump.onCrouchLand += StartSlide;
        
        ConveyorBelt.onStartUsing += StopSlide;
        
        PCamFOV.AddFOV(slideFOV);
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
        
        if(rb.velocity.magnitude < slideStopSpeedThreshold || !performSlideConditions.ConditionsMet()) StopSlide();
        
        slideFOV.SetNewAmount(slideAddedFOV + rb.velocity.magnitude * slideAddedFOVPerVelocity);
    }

    private void Slide()
    {
        PlayerSlideCalculator.ApplySlideDamping(rb.velocity,slideDampingProperties);
    }
    // public bool CanStartAction() => PGrounded.IsGrounded &&
    //                                 !Sliding &&
    //                                 Vector2.Dot(InputManager.MoveInput,Vector2.up) > 0.2f &&
    //                                 !SItem.Sleighing &&
    //                                 !PRagdoll.Ragdolling;
    //
    // private bool CanPerformAction() => !SItem.Sleighing && 
    //                                    PJump.CanJump &&
    //                                    !PJump.Jumping &&
    //                                     &&
    //                                    !PRagdoll.Ragdolling;
    public void StartSlide()
    {
        if (!startSlideConditions.ConditionsMet()) return;
        
        Debug.Log("Sliding");
        Sliding = true;
        pHeight.SetHeight(PHeight.HeightType.Crouch);
        slideFOV.SetNewAmount(slideAddedFOV);

        if (PGrounded.IsOnControllableSlope)
        {
            Vector3 force = orientation.forward * startSlideForce;
            PMovementCalculator.GetGroundProjectedVector(force);
            rb.AddForce(force,ForceMode.Impulse);
        }

        Slide();
    }

    public void StopSlide()
    {
        if (!Sliding) return;
        
        Sliding = false;
        PDamping.ResetDamping();
        if(!SItem.Sleighing) pHeight.SetHeight(PHeight.HeightType.Normal);
        slideFOV.SetNewAmount(0);
    }
}