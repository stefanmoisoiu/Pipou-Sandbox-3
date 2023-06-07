using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PlayerCrouch : NetworkBehaviour,IPlayerCrouchAction
{
    
    
    public static bool Crouching { get; private set; }
    [SerializeField]private PlayerHeight playerHeight;

    private void Start()
    {
        if (!IsOwner) return;
        ConveyorBelt.onStartUsing += StopAction;
    }
    public bool CanStartAction() => PlayerGrounded.IsGrounded &&
                                    !Crouching &&
                                    !SleighItem.Sleighing &&
                                    !ConveyorBelt.SuperSpeedConveyor;
    private bool CanPerformAction() => PlayerGrounded.IsGrounded && !SleighItem.Sleighing;
    public void StartAction()
    {
        Crouching = true;
        playerHeight.SetHeight(PlayerHeight.HeightType.Crouch);
    }

    private void Update()
    {
        if(!CanPerformAction()) StopAction();
    }

    public void StopAction()
    {
        if (!Crouching) return;
        Crouching = false;
        if(!SleighItem.Sleighing)playerHeight.SetHeight(PlayerHeight.HeightType.Normal);
    }


}