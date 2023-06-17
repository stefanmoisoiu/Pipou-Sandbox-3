using System;
using Unity.Netcode;
using UnityEngine;

public class PDive : NetworkBehaviour
{
    [SerializeField] private InputBind startDiveBind;
    [SerializeField] private ActionConditions startDiveConditions,performDiveConditions;
    
    [SerializeField] private float forwardDiveForce,upDiveForce;
    [Range(0,1)] [SerializeField]
    private float diveDamping = 0.975f;

    [SerializeField] private PCamFOV.FadingFOV diveFOV;

    
    [SerializeField] private Transform orientation;

    [SerializeField]private Rigidbody rb;
    [SerializeField]private PHeight pHeight;

    public static bool Diving { get; private set; }
    public static Action onStopDive,onStopDiveCrouch;
    private void Start()
    {
        if (!IsOwner) return;
        startDiveBind.Bind();
        ConveyorBelt.onStartUsing += StopDive;
        PWallClimb.onStartWallClimb += StopDive;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if(Diving) PDamping.SetDamping(diveDamping);
        if (!performDiveConditions.ConditionsMet()) StopDive();
        // if (Diving && PGrounded.IsGrounded && PGrounded.IsOnControllableSlope) StopDive();
    }
    // public bool CanStartAction() => !Diving &&
    //                                 !PGrounded.IsGrounded &&
    //                                 !PWallClimb.WallClimbing &&
    //                                 !SItem.Sleighing &&
    //                                 !ConveyorBelt.SuperSpeedConveyor &&
    //                                 !PRagdoll.Ragdolling;
    public void StartDive()
    {
        if (!startDiveConditions.ConditionsMet()) return;
        Diving = true;
        PDamping.SetDamping(diveDamping); 
        PCamFOV.AddFOV(PCamFOV.FadingFOV.Copy(diveFOV));
        pHeight.SetHeight(PHeight.HeightType.Crouch);
        
        rb.velocity = orientation.forward * forwardDiveForce + orientation.up * upDiveForce;
    }
    
    private void StopDive()
    {
        if (!Diving) return;
        Diving = false;
        PDamping.ResetDamping();
        pHeight.SetHeight(PHeight.HeightType.Normal);
        onStopDive?.Invoke();
        if(InputManager.PressingCrouch) onStopDiveCrouch?.Invoke();
    }
}
