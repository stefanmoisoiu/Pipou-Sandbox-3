using Unity.Netcode;
using UnityEngine;

public class PCrouch : NetworkBehaviour
{
    [SerializeField] private InputBind startCrouchBind,stopCrouchBind;
    [SerializeField] private ActionConditions startCrouchConditions,performCrouchConditions;
    
    
    public static bool Crouching { get; private set; }
    [SerializeField]private PHeight pHeight;

    private void Start()
    {
        if (!IsOwner) return;
        startCrouchBind.Bind();
        stopCrouchBind.Bind();
        
        PDive.onStopDiveCrouch += StartCrouch;
        PJump.onCrouchLand += StartCrouch;
        
        ConveyorBelt.onStartUsing += StopCrouch;
    }
    // public bool CanStartAction() => PGrounded.IsGrounded &&
    //                                 !Crouching &&
    //                                 !SItem.Sleighing &&
    //                                 !ConveyorBelt.SuperSpeedConveyor &&
    //                                 !PRagdoll.Ragdolling;
    // private bool CanPerformAction() => PGrounded.IsGrounded && !SItem.Sleighing;
    private void Update()
    {
        if (!performCrouchConditions.ConditionsMet()) StopCrouch();
    }
    public void StartCrouch()
    {
        if (!startCrouchConditions.ConditionsMet()) return;
        Debug.Log("Crouching");
        Crouching = true;
        pHeight.SetHeight(PHeight.HeightType.Crouch);
    }
    public void StopCrouch()
    {
        if (!Crouching) return;
        Crouching = false;
        if(!SItem.Sleighing)pHeight.SetHeight(PHeight.HeightType.Normal);
    }
}