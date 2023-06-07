using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    private static Controls controls;

    public static Vector2 MoveInput,LookInput;

    public static Action 
        onJump,whileJump,onStopJump, 
        onCrouch,whileCrouch,onStopCrouch,
        onRun,whileRun,onStopRun,
        onUse,whileUse,onStopUse;
    public static Action<int> onSelectItem;

    public static bool PressingJump, PressingCrouch, PressingRun,PressingUse;

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
