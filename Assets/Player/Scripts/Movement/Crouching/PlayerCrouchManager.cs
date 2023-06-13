using Unity.Netcode;
using UnityEngine;

public class PlayerCrouchManager : NetworkBehaviour
{
    [SerializeField] private Component[] crouchActions;

    private void Start()
    {
        if (!IsOwner) return;
        InputManager.onCrouch += StartCrouchAction;
        InputManager.onStopCrouch += StopCrouchAction;
        PRagdoll.onSetRagdoll += delegate(bool value) { if(value) StopCrouchAction(); };
    }

    /// <summary>
    /// Called on crouch press, manages all scripts using the crouch key and chooses the first in the list it can do.
    /// Crouch scripts ex: PlayerDive, PlayerSlide, PlayerCrouch
    /// </summary>
    public void StartCrouchAction()
    {
        foreach (var crouchAction in crouchActions)
        {
            if (((IPlayerCrouchAction)crouchAction).CanStartAction())
            {
                ((IPlayerCrouchAction)crouchAction).StartAction();
                return;
            }
        }
    }
    /// <summary>
    /// Stops all crouch scripts
    /// </summary>
    public void StopCrouchAction()
    {
        foreach (var crouchAction in crouchActions)
            ((IPlayerCrouchAction)crouchAction).StopAction();
    }
}
