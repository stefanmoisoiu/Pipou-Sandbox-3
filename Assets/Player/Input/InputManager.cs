using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    private static Controls controls;

    public static Vector2 MoveInput,LookInput;

    public static ActionCallback
        onJump,whileJump,onStopJump, 
        onCrouch,whileCrouch,onStopCrouch,
        onRun,whileRun,onStopRun,
        onUse,whileUse,onStopUse;
    public delegate void ActionCallback();

    private static ActionCallback[] jump;
    private static ActionCallback[] crouch;
    private static ActionCallback[] run;
    private static ActionCallback[] use;
    public static Action<int> onSelectItem;

    public static bool PressingJump, PressingCrouch, PressingRun,PressingUse;

    public enum ActionType {Jump,Crouch,Run,Use}
    public enum ActionAdvancement {Start,Performed,Finished}

    public static void Bind(ActionCallback actionCallback,ActionType actionType, ActionAdvancement actionAdvancement)
    {
        switch (actionAdvancement)
        {
            case ActionAdvancement.Start: 
                switch (actionType)
                {
                    case ActionType.Crouch: onCrouch += actionCallback;
                    break;
                    case ActionType.Jump: onJump += actionCallback;
                    break;
                    case ActionType.Run: onRun += actionCallback;
                    break;
                    case ActionType.Use: onUse += actionCallback;
                    break;
                }
                break;
            case ActionAdvancement.Performed: 
                switch (actionType)
                {
                    case ActionType.Crouch: whileCrouch += actionCallback;
                        break;
                    case ActionType.Jump: whileJump += actionCallback;
                    break;
                    case ActionType.Run: whileRun += actionCallback;
                    break;
                    case ActionType.Use: whileUse += actionCallback;
                    break;
                }
                break;
            case ActionAdvancement.Finished: 
                switch (actionType)
                {
                    case ActionType.Crouch: onStopCrouch += actionCallback;
                    break;
                    case ActionType.Jump: onStopJump += actionCallback;
                    break;
                    case ActionType.Run: onStopRun += actionCallback;
                    break;
                    case ActionType.Use: onStopUse += actionCallback;
                    break;
                }
                break;
        }
    }
    public void Start()
    {
        if (!IsOwner) return;
        if (controls != null) return;
        
        controls = new();
        controls.Enable();
        
        controls.Movement.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        controls.Movement.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();

        
        controls.Movement.Jump.started += ctx => onJump?.Invoke();
        controls.Movement.Jump.canceled += ctx => onStopJump?.Invoke();

        controls.Movement.Jump.started += ctx => PressingJump = true;
        controls.Movement.Jump.canceled += ctx => PressingJump = false;
        
        
        controls.Movement.Crouch.started += ctx => onCrouch?.Invoke();
        controls.Movement.Crouch.performed += ctx => whileCrouch?.Invoke();
        controls.Movement.Crouch.canceled += ctx => onStopCrouch?.Invoke();
        
        controls.Movement.Crouch.started += ctx => PressingCrouch = true;
        controls.Movement.Crouch.canceled += ctx => PressingCrouch = false;
        
        
        controls.Movement.Run.started += ctx => onRun?.Invoke();
        controls.Movement.Run.performed += ctx => whileRun?.Invoke();
        controls.Movement.Run.canceled += ctx => onStopRun?.Invoke();
        
        controls.Movement.Run.started += ctx => PressingRun = true;
        controls.Movement.Run.canceled += ctx => PressingRun = false;

        controls.Actions.Item1.started += ctx => onSelectItem?.Invoke(0);
        controls.Actions.Item2.started += ctx => onSelectItem?.Invoke(1);
        controls.Actions.Item3.started += ctx => onSelectItem?.Invoke(2);
        
        controls.Actions.Use.started += ctx => onUse?.Invoke();
        controls.Actions.Use.performed += ctx => whileUse?.Invoke();
        controls.Actions.Use.canceled += ctx => onStopUse?.Invoke();
        
        controls.Actions.Use.started += ctx => PressingUse = true;
        controls.Actions.Use.canceled += ctx => PressingUse = false;
    }
    private void Update()
    {
        if(PressingJump) whileJump?.Invoke();
        if(PressingCrouch) whileCrouch?.Invoke();
        if(PressingRun) whileRun?.Invoke();
        if(PressingUse) whileUse?.Invoke();
    }

    private void OnEnable()
    {
        if(IsOwner && controls != null) controls.Enable();
    }
    private void OnDisable()
    {
        if(IsOwner && controls != null) controls.Disable();
    }
}
