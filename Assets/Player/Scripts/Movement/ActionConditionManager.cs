using System;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct ActionConditions
{
    public enum ConditionCheckType {And,Or}
    [SerializeField] private ConditionCheckType checkType;
    
    [SerializeField] private ActionCondition[] actionConditions;
        
    public bool ConditionsMet()
    {
        if (checkType == ConditionCheckType.And)
        {
            foreach (ActionCondition actionCondition in actionConditions) if (!actionCondition.ConditionMet()) return false;
            return true;
        }
        else if (checkType == ConditionCheckType.Or)
        {
            foreach (ActionCondition actionCondition in actionConditions) if (actionCondition.ConditionMet()) return true;
            return false;
        }
        Debug.LogError("Condition Type Not Found");
        return false;
    }
}
[Serializable]
public struct ActionCondition
{
    public enum ConditionType {
        TooFast,
        IsGrounded,
        IsOnControllableGround,
        Jumping,
        CanJump,
        WallClimbing,
        WallClimbPushing,
        Crouching,
        Sliding,
        Diving,
        NormalHeight,
        CrouchHeight,
        Ragdolling,
        Talking,
        Sleighing,
        UsingConveyorBelt,
        CanWallClimb,
        MovingForward,
        LassoThrown,
        Lassoed,
        ChargingLasso
    }

    [HorizontalGroup("H")] public ConditionType conditionType;
    [HorizontalGroup("H")] public bool value;

    public bool ConditionMet()
    {
        switch (conditionType)
        {
            case ConditionType.TooFast: return PMovement.TooFast == value;
            case ConditionType.MovingForward: return PMovement.MovingForward == value;
            
            case ConditionType.IsGrounded: return PGrounded.IsGrounded == value;
            case ConditionType.IsOnControllableGround: return PGrounded.IsOnControllableSlope == value;
            
            case ConditionType.Jumping: return PJump.Jumping == value;
            case ConditionType.CanJump: return PJump.CanJump == value;
            
            case ConditionType.WallClimbing: return PWallClimb.WallClimbing == value;
            case ConditionType.WallClimbPushing: return PWallClimb.WallClimbPushing == value;
            case ConditionType.CanWallClimb: return PWallClimb.CanWallClimb == value;
            
            case ConditionType.Crouching: return PCrouch.Crouching == value;
            
            case ConditionType.Sliding: return PSlide.Sliding == value;
            
            case ConditionType.Diving: return PDive.Diving == value;
            
            case ConditionType.NormalHeight: return PHeight.NormalHeight == value;
            case ConditionType.CrouchHeight: return PHeight.CrouchHeight == value;
            
            case ConditionType.Ragdolling: return PRagdoll.Ragdolling == value;
            
            case ConditionType.Talking: return DialogueManager.Talking == value;
            
            case ConditionType.Sleighing: return SItem.Sleighing == value;
            
            case ConditionType.UsingConveyorBelt: return ConveyorBelt.SuperSpeedConveyor == value;
            
            case ConditionType.ChargingLasso: return LassoItem.ChargingLasso == value;
            case ConditionType.LassoThrown: return LassoItem.LassoThrown == value;
            case ConditionType.Lassoed: return LassoItem.Lassoed == value;
        }
        Debug.LogError("Could not find Condition");
        return false;
    }
}