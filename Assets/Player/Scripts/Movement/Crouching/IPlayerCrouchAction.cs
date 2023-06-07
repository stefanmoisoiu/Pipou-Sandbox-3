using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used in every script using the crouch key for an action.
/// </summary>
public interface IPlayerCrouchAction
{
    public bool CanStartAction();
    public void StartAction();
    public void StopAction();
}
