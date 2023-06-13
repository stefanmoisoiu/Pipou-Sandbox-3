using Unity.Netcode;
using UnityEngine;

public class PAnimator : NetworkBehaviour
{
    private static readonly int MovementX = Animator.StringToHash("MovementX");
    private static readonly int MovementY = Animator.StringToHash("MovementY");

    [SerializeField]private Animator animator;
    [SerializeField] private float animMoveLerpSpeed = 15,animTransitionDuration = 0.15f;
    private Vector2 animMoveInput;

    private ushort currentBodyAnimIndex,currentHandAnimIndex;
    private NetworkVariable<float> targetMovementX = new(writePerm:NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> targetMovementY = new(writePerm:NetworkVariableWritePermission.Owner);
    
    private NetworkVariable<ushort> currentBodyAnim = new(writePerm:NetworkVariableWritePermission.Owner);
    private NetworkVariable<ushort> currentHandAnim = new(writePerm:NetworkVariableWritePermission.Owner);
    private static string[] _bodyAnims = {"Movement","Jumping","Falling","Crouch","Slide","Dive","Wall Climb","Treadmill"};
    private static string[] _handAnims = {"Empty Hands","Lasso Hold","Lasso Charge","Lasso Throw","Boxing","Cross Punch"};
    private static bool[] _handAnimsTransitionSelf = {false,false,false,false,false,true};
    private void Update()
    {
        if (IsOwner)
        {
            SetMovementAnims();
            PlayBodyAnim(GetMovementAnim());
        }
        else
        {
            SetNetworkAnims();
            PlayNetworkAnims();
        }
    }

    private ushort GetMovementAnim()
    {
        if (ConveyorBelt.SuperSpeedConveyor) return 7;
        if (PDive.Diving) return 5;
        if (PWallClimb.WallClimbing) return 6;
        if (PJump.Jumping) return 1;
        if (PCrouch.Crouching) return 3;
        if (PSlide.Sliding || SItem.Sleighing) return 4;
        if (PGrounded.IsGrounded) return 0;
        return 2;
    }
    private void SetMovementAnims()
    {
        Vector2 moveInput = InputManager.MoveInput;
        targetMovementX.Value = moveInput.x;
        targetMovementY.Value = moveInput.y;
        animMoveInput = Vector2.Lerp(animMoveInput, moveInput, animMoveLerpSpeed * Time.deltaTime);
        animator.SetFloat(MovementX,animMoveInput.x);
        animator.SetFloat(MovementY,animMoveInput.y);
    }

    private void PlayBodyAnim(ushort animIndex)
    {
        if (currentBodyAnimIndex == animIndex) return;
        
        animator.CrossFadeInFixedTime(_bodyAnims[animIndex],animTransitionDuration);
        
        currentBodyAnim.Value = animIndex;
        currentBodyAnimIndex = animIndex;
    }

    public void PlayHandAnim(int animIndex) => PlayHandAnim((ushort)animIndex);
    public void PlayHandAnim(ushort animIndex)
    {
        if (currentHandAnimIndex == animIndex && !_handAnimsTransitionSelf[animIndex]) return;
        
        animator.CrossFadeInFixedTime(_handAnims[animIndex],animTransitionDuration);
        
        if (_handAnimsTransitionSelf[animIndex] && currentHandAnimIndex == animIndex)
            ForcePlayHandAnimServerRpc(animIndex);
        
        currentHandAnim.Value = animIndex;
        currentHandAnimIndex = animIndex;
    }
    private void SetNetworkAnims()
    {
        animMoveInput = Vector2.Lerp(animMoveInput, new Vector2(targetMovementX.Value,targetMovementY.Value), animMoveLerpSpeed * Time.deltaTime);
        animator.SetFloat(MovementX,animMoveInput.x);
        animator.SetFloat(MovementY,animMoveInput.y);
    }
    private void PlayNetworkAnims()
    {
        if (currentBodyAnimIndex != currentBodyAnim.Value)
        {
            animator.CrossFadeInFixedTime(_bodyAnims[currentBodyAnim.Value],animTransitionDuration);
            currentBodyAnimIndex = currentBodyAnim.Value;
        }
        if (currentHandAnimIndex != currentHandAnim.Value)
        {
            animator.CrossFadeInFixedTime(_handAnims[currentHandAnim.Value],animTransitionDuration);
            currentHandAnimIndex = currentHandAnim.Value;
        }
    }

    [ServerRpc]
    private void ForcePlayHandAnimServerRpc(ushort index) => ForcePlayHandAnimClientRpc(index);
    [ClientRpc]
    private void ForcePlayHandAnimClientRpc(ushort index)
    {
        animator.CrossFadeInFixedTime(_handAnims[index],animTransitionDuration);
    }
}
