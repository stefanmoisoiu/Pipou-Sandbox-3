using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PCrouch : NetworkBehaviour,IPlayerCrouchAction
{
    
    
    public static bool Crouching { get; private set; }
    [SerializeField]private PHeight pHeight;

    private void Start()
    {
        if (!IsOwner) return;
        ConveyorBelt.onStartUsing += StopAction;
    }
    public bool CanStartAction() => PGrounded.IsGrounded &&
                                    !Crouching &&
                                    !SItem.Sleighing &&
                                    !ConveyorBelt.SuperSpeedConveyor &&
                                    !PRagdoll.Ragdolling;
    private bool CanPerformAction() => PGrounded.IsGrounded && !SItem.Sleighing;
    public void StartAction()
    {
        Crouching = true;
        pHeight.SetHeight(PHeight.HeightType.Crouch);
    }

    private void Update()
    {
        if(!CanPerformAction()) StopAction();
    }

    public void StopAction()
    {
        if (!Crouching) return;
        Crouching = false;
        if(!SItem.Sleighing)pHeight.SetHeight(PHeight.HeightType.Normal);
    }


}