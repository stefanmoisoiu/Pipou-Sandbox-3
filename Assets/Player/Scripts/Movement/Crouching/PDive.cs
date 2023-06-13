using System;
using Unity.Netcode;
using UnityEngine;

public class PDive : NetworkBehaviour,IPlayerCrouchAction
{
    [SerializeField] private float forwardDiveForce,upDiveForce;
    [Range(0,1)] [SerializeField]
    private float diveDamping = 0.975f;

    [SerializeField] private PCamFOV.FadingFOV diveFOV;

    
    [SerializeField] private Transform orientation;

    [SerializeField]private Rigidbody rb;
    [SerializeField]private PHeight pHeight;
    [SerializeField]private PlayerCrouchManager playerCrouchManager;

    public static bool Diving { get; private set; }

    private void Start()
    {
        ConveyorBelt.onStartUsing += StopDive;
        PWallClimb.onStartWallClimb += StopDive;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if(Diving) PDamping.SetDamping(diveDamping);
        if (Diving && PGrounded.IsGrounded && PGrounded.IsOnControllableSlope) StopDive();
    }
    public bool CanStartAction() => !Diving &&
                                    !PGrounded.IsGrounded &&
                                    !PWallClimb.WallClimbing &&
                                    !SItem.Sleighing &&
                                    !ConveyorBelt.SuperSpeedConveyor &&
                                    !PRagdoll.Ragdolling;
    public void StartAction()
    {
        Diving = true;
        PDamping.SetDamping(diveDamping); 
        PCamFOV.AddFOV(PCamFOV.FadingFOV.Copy(diveFOV));
        pHeight.SetHeight(PHeight.HeightType.Crouch);
        
        rb.velocity = orientation.forward * forwardDiveForce + orientation.up * upDiveForce;
    }

    public void StopAction()
    {
        //Stopping key press doesnt stop dive
    }
    
    public void StopDive()
    {
        if (!Diving) return;
        Diving = false;
        PDamping.ResetDamping();
        pHeight.SetHeight(PHeight.HeightType.Normal);
        if(InputManager.PressingCrouch) playerCrouchManager.StartCrouchAction();
    }
}
