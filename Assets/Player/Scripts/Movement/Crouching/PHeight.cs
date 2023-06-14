using System;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;

public class PHeight : NetworkBehaviour
{
    public enum HeightType {Normal,Crouch}

    private float baseHeight,baseCenter;
     
    [FoldoutGroup("Crouch Height Property")] [SerializeField] private float crouchHeight;
    
    [FoldoutGroup("Stop Crouch Properties")] [SerializeField]
    private Transform stopCrouchCheckPos;
    [FoldoutGroup("Stop Crouch Properties")] [SerializeField]
    private float stopCrouchCheckSize;
    [FoldoutGroup("Stop Crouch Properties")] [SerializeField]
    private LayerMask stopCrouchLayer;
    [FoldoutGroup("Height Lerp Properties")] [SerializeField]
    private float heightLerpDuration;
    [FoldoutGroup("Height Lerp Properties")] [SerializeField]
    private AnimationCurve heightLerpEase;
    


    public static HeightType CurrentHeight { get; private set; }
    private HeightType targetHeight;
    public static Action<HeightType> onHeightChange;

    private Coroutine heightCoroutine;

    [SerializeField]private CapsuleCollider capsuleCollider;
    [SerializeField]private Rigidbody rb;
    
    public static bool NormalHeight { get; private set; }
    public static bool CrouchHeight { get; private set; }
    private void Start()
    {
        if (!IsOwner) return;
        baseHeight = capsuleCollider.height;
        baseCenter = capsuleCollider.center.y;
        ConveyorBelt.onStartUsing += SetNormalHeight;
        PRagdoll.onSetRagdoll += delegate(bool value) { if(value) SetNormalHeight(); };
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (targetHeight != CurrentHeight) SetHeight(targetHeight);
    }

    public void SetNormalHeight() => SetHeight(HeightType.Normal);
    public void SetCrouchHeight() => SetHeight(HeightType.Crouch);
    public void SetHeight(HeightType heightType)
    {
        targetHeight = heightType;
        switch (heightType)
        {
            case HeightType.Normal:
                if (CurrentHeight != HeightType.Normal && Physics.CheckSphere(stopCrouchCheckPos.position, stopCrouchCheckSize,stopCrouchLayer)) return;
                
                if(heightCoroutine != null) StopCoroutine(heightCoroutine);
                heightCoroutine = StartCoroutine(HeightLerp(baseHeight));
                NormalHeight = true;
                CrouchHeight = false;
                break;
            case HeightType.Crouch:
                if(heightCoroutine != null) StopCoroutine(heightCoroutine);
                heightCoroutine = StartCoroutine(HeightLerp(crouchHeight));
                NormalHeight = false;
                CrouchHeight = true;
                break;
        }
        onHeightChange?.Invoke(heightType);
        CurrentHeight = heightType;
    }

    private IEnumerator HeightLerp(float target)
    {
        float advancement = 0;
        float startCapsuleHeight = capsuleCollider.height;
        
        while (advancement < 1)
        {
            float deltaHeight = capsuleCollider.height;
            float deltaCenter = capsuleCollider.center.y;
            
            capsuleCollider.height = Mathf.Lerp(startCapsuleHeight,target, heightLerpEase.Evaluate(advancement));
            capsuleCollider.center = Vector3.up * (baseHeight - Mathf.Lerp(startCapsuleHeight,target, heightLerpEase.Evaluate(advancement)))/2 + Vector3.up * baseCenter;

            deltaHeight = capsuleCollider.height - deltaHeight;
            deltaCenter = capsuleCollider.center.y - deltaCenter;

            if (PGrounded.IsGrounded && PJump.CanJump)
            {
                rb.MovePosition(new (rb.position.x, rb.position.y + (deltaHeight + deltaCenter)*2f, rb.position.z));
            }
            advancement += Time.deltaTime / heightLerpDuration;
            yield return null;
        }

        capsuleCollider.height = target;
        capsuleCollider.center = Vector3.up * (baseCenter + (baseHeight - target) / 2);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(stopCrouchCheckPos.position,stopCrouchCheckSize);
    }
}
