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

    public static InputCallback
        onJump,whileJump,onStopJump, 
        onCrouch,whileCrouch,onStopCrouch,
        onRun,whileRun,onStopRun,
        onUse,whileUse,onStopUse,
        onSecondaryUse,whileSecondaryUse,onStopSecondaryUse;
    public delegate void InputCallback();

    private static InputCallback[] jump;
    private static InputCallback[] crouch;
    private static InputCallback[] run;
    private static InputCallback[] use;
    private static InputCallback[] secondaryUse;
    public static Action<int> onSelectItem;

    public static bool PressingJump, PressingCrouch, PressingRun,PressingUse,PressingSecondaryUse;

    public enum InputType {Jump,Crouch,Run,Use,SecondaryUse}
    public enum InputAdvancement {Start,Performed,Finished}

    public static void Bind(InputCallback inputCallback,InputType inputType, InputAdvancement inputAdvancement)
    {
        switch (inputAdvancement)
        {
            case InputAdvancement.Start: 
                switch (inputType)
                {
                    case InputType.Crouch: onCrouch += inputCallback; break;
                    case InputType.Jump: onJump += inputCallback; break;
                    case InputType.Run: onRun += inputCallback; break;
                    case InputType.Use: onUse += inputCallback; break;
                    case InputType.SecondaryUse: onSecondaryUse += inputCallback; break;
                }
                break;
            case InputAdvancement.Performed: 
                switch (inputType)
                {
                    case InputType.Crouch: whileCrouch += inputCallback; break;
                    case InputType.Jump: whileJump += inputCallback; break;
                    case InputType.Run: whileRun += inputCallback; break;
                    case InputType.Use: whileUse += inputCallback; break;
                    case InputType.SecondaryUse: whileSecondaryUse += inputCallback; break;
                }
                break;
            case InputAdvancement.Finished: 
                switch (inputType)
                {
                    case InputType.Crouch: onStopCrouch += inputCallback; break;
                    case InputType.Jump: onStopJump += inputCallback; break;
                    case InputType.Run: onStopRun += inputCallback; break;
                    case InputType.Use: onStopUse += inputCallback; break;
                    case InputType.SecondaryUse: onStopSecondaryUse += inputCallback; break;
                }
                break;
        }
    }
    public static void UnBind(InputCallback inputCallback,InputType inputType, InputAdvancement inputAdvancement)
    {
        switch (inputAdvancement)
        {
            case InputAdvancement.Start: 
                switch (inputType)
                {
                    case InputType.Crouch: onCrouch -= inputCallback; break;
                    case InputType.Jump: onJump -= inputCallback; break;
                    case InputType.Run: onRun -= inputCallback; break;
                    case InputType.Use: onUse -= inputCallback; break;
                    case InputType.SecondaryUse: onSecondaryUse -= inputCallback; break;
                }
                break;
            case InputAdvancement.Performed: 
                switch (inputType)
                {
                    case InputType.Crouch: whileCrouch -= inputCallback; break;
                    case InputType.Jump: whileJump -= inputCallback; break;
                    case InputType.Run: whileRun -= inputCallback; break;
                    case InputType.Use: whileUse -= inputCallback; break;
                    case InputType.SecondaryUse: whileSecondaryUse -= inputCallback; break;
                }
                break;
            case InputAdvancement.Finished: 
                switch (inputType)
                {
                    case InputType.Crouch: onStopCrouch -= inputCallback; break;
                    case InputType.Jump: onStopJump -= inputCallback; break;
                    case InputType.Run: onStopRun -= inputCallback; break;
                    case InputType.Use: onStopUse -= inputCallback; break;
                    case InputType.SecondaryUse: onStopSecondaryUse -= inputCallback; break;
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
        
        controls.Actions.SecondaryUse.started += ctx => onSecondaryUse?.Invoke();
        controls.Actions.SecondaryUse.performed += ctx => whileSecondaryUse?.Invoke();
        controls.Actions.SecondaryUse.canceled += ctx => onStopSecondaryUse?.Invoke();
        
        controls.Actions.SecondaryUse.started += ctx => PressingSecondaryUse = true;
        controls.Actions.SecondaryUse.canceled += ctx => PressingSecondaryUse = false;
    }
    private void Update()
    {
        if(PressingJump) whileJump?.Invoke();
        if(PressingCrouch) whileCrouch?.Invoke();
        if(PressingRun) whileRun?.Invoke();
        if(PressingUse) whileUse?.Invoke();
        if(PressingSecondaryUse) whileSecondaryUse?.Invoke();
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
