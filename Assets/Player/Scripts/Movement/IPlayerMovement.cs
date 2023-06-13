using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public abstract class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private MovementCondition[] startMovementConditions;

    internal bool CanStartMovement()
    {
        foreach (MovementCondition movementCondition in startMovementConditions)
        {
            if (!movementCondition.ConditionMet()) return false;
        }

        return true;
    }
}

[Serializable]
public struct MovementCondition
{
    public enum ConditionType {
        TooFast,
        IsGrounded, IsOnControllableGround,
        Jumping,CanJump,
        WallClimbing,WallClimbPushing,
        Crouching,Sliding,Diving,
        NormalHeight,CrouchHeight,
        Ragdolling,
        Talking,
        Sleighing
    }

    [HorizontalGroup("H")] public ConditionType conditionType;
    [HorizontalGroup("H")] public bool value;

    public bool ConditionMet()
    {
        switch (conditionType)
        {
            case ConditionType.TooFast: return PMovement.TooFast == value;
            
            case ConditionType.IsGrounded: return PGrounded.IsGrounded == value;
            case ConditionType.IsOnControllableGround: return PGrounded.IsOnControllableSlope == value;
            
            case ConditionType.Jumping: return PJump.Jumping == value;
            case ConditionType.CanJump: return PJump.CanJump == value;
            
            case ConditionType.WallClimbing: return PWallClimb.WallClimbing == value;
            case ConditionType.WallClimbPushing: return PWallClimb.WallClimbPushing == value;
            
            case ConditionType.Crouching: return PCrouch.Crouching == value;
            
            case ConditionType.Sliding: return PSlide.Sliding == value;
            
            case ConditionType.Diving: return PDive.Diving == value;
            
            case ConditionType.NormalHeight: return PHeight.NormalHeight == value;
            case ConditionType.CrouchHeight: return PHeight.CrouchHeight == value;
            
            case ConditionType.Ragdolling: return PRagdoll.Ragdolling == value;
            
            case ConditionType.Talking: return DialogueManager.Talking == value;
            
            case ConditionType.Sleighing: return SItem.Sleighing == value;
        }
        Debug.LogError("Could not find Condition");
        return false;
    }
}