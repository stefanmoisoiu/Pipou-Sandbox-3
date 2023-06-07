using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerDive : NetworkBehaviour,IPlayerCrouchAction
{
    [SerializeField] private float forwardDiveForce,upDiveForce;
    [Range(0,1)] [SerializeField]
    private float diveDamping = 0.975f;

    [SerializeField] private PlayerCamFOV.FadingFOV diveFOV;

    
    [SerializeField] private Transform orientation;

    [SerializeField]private Rigidbody rb;
    [SerializeField]private PlayerHeight playerHeight;
    [SerializeField]private PlayerCrouchManager playerCrouchManager;

    public static bool Diving { get; private set; }

    private void Start()
    {
        ConveyorBelt.onStartUsing += StopDive;
        PlayerWallClimb.onStartWallClimb += StopDive;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if(Diving) PlayerDamping.SetDamping(diveDamping);
        if (Diving && PlayerGrounded.IsGrounded) StopDive();
    }
    public bool CanStartAction() => !Diving &&
                                    !PlayerGrounded.IsGrounded &&
                                    !PlayerWallClimb.WallClimbing &&
                                    !SleighItem.Sleighing &&
                                    !ConveyorBelt.SuperSpeedConveyor;
    public void StartAction()
    {
        Diving = true;
        PlayerDamping.SetDamping(diveDamping); 
        PlayerCamFOV.AddFOV(PlayerCamFOV.FadingFOV.Copy(diveFOV));
        playerHeight.SetHeight(PlayerHeight.HeightType.Crouch);
        
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
        PlayerDamping.ResetDamping();
        playerHeight.SetHeight(PlayerHeight.HeightType.Normal);
        if(InputManager.PressingCrouch) playerCrouchManager.StartCrouchAction();
    }
}
