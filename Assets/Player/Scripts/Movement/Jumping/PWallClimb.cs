using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PWallClimb : NetworkBehaviour
{
    [SerializeField] private InputBind startWallClimbBind,stopWallClimbBind;
    
    [SerializeField] private ActionConditions startWallClimbConditions,performWallClimbConditions;
    [SerializeField] private float
        wallClimbLength = 0.6f, wallClimbAngleMargin = 30, wallClimbSpeed = 0.1f, maxWallClimbSpeed = 4,
        startWallCheckDistance = 1,wallCheckDistance = 1.75f,
        wallPushHSpeed = 3f,wallPushVSpeed=2f,wallPushDuration = 0.5f;
    
    [SerializeField] private LayerMask groundMask;
    private float currentWallClimbDuration = 0;
    [SerializeField] private Transform feetPos;
    [SerializeField] private CapsuleCollider col;
    
    public static bool WallClimbing { get; private set; }
    public static bool WallClimbPushing { get; private set; }
    public static bool CanWallClimb { get; private set; }

    [SerializeField]private Rigidbody rb;
    [SerializeField]private PHeight pHeight;

    public static Action onStartWallClimb, onStopWallClimb;
    private void Start()
    {
        if (!IsOwner) return;
        CanWallClimb = true;
        startWallClimbBind.Bind();
        stopWallClimbBind.Bind();
        // InputManager.whileJump += StartWallClimb;
        // InputManager.onStopJump += delegate { StopWallClimb(true); };
        ConveyorBelt.onStartUsing += delegate { StopWallClimb(false); };
        PRagdoll.onSetRagdoll += delegate(bool value) { if(value) StopWallClimb(false); };
        
    }

    private void Update()
    {
        if (!IsOwner) return;
        if(PGrounded.IsGrounded && PGrounded.IsOnControllableSlope) CanWallClimb = true;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (!WallClimbing) return;
        rb.velocity = new Vector3(0, Mathf.Min(rb.velocity.y + wallClimbSpeed,maxWallClimbSpeed),0);
        currentWallClimbDuration += Time.fixedDeltaTime;
        
        if(!Physics.Raycast(feetPos.position, feetPos.forward,out RaycastHit hit, col.radius * transform.root.localScale.x + wallCheckDistance,
               groundMask)) StopWallClimb(true);
        
        if(currentWallClimbDuration > wallClimbLength || !performWallClimbConditions.ConditionsMet()) StopWallClimb(true);
    }

    // private bool CanStopWallClimb() => currentWallClimbDuration > wallClimbLength ||
    //                                    SItem.Sleighing;
    // private bool CanStartWallClimb() => CanWallClimb &&
    //                                     !WallClimbing &&
    //                                     !PGrounded.IsGrounded &&
    //                                     !SItem.Sleighing &&
    //                                     !ConveyorBelt.SuperSpeedConveyor &&
    //                                     !PRagdoll.Ragdolling;
        
    public void StartWallClimb()
    {
        if (!startWallClimbConditions.ConditionsMet()) return;
        if (!Physics.Raycast(feetPos.position, feetPos.forward, out RaycastHit hit, col.radius * transform.root.localScale.x + startWallCheckDistance,groundMask)) return;
        if (Vector3.Angle(new Vector3(hit.normal.x, 0, hit.normal.z).normalized, hit.normal) >
            wallClimbAngleMargin / 2) return;
        onStartWallClimb?.Invoke();
        CanWallClimb = false;
        pHeight.SetHeight(PHeight.HeightType.Normal);
        rb.useGravity = false;
        WallClimbing = true;
        currentWallClimbDuration = 0;
    }

    public void StopWallClimb(bool tryWallClimbPush)
    {
        if (!WallClimbing) return;
        WallClimbing = false;
        onStopWallClimb?.Invoke();
        
        rb.useGravity = true;
        rb.velocity /= 2;
        if(tryWallClimbPush)WallClimbPush();
    }
    private void WallClimbPush()
    {
        if (Physics.Raycast(feetPos.position, feetPos.forward,out RaycastHit hit, col.radius * transform.root.localScale.x + wallCheckDistance, groundMask))
        {
            rb.velocity = hit.normal * wallPushHSpeed + Vector3.up * wallPushVSpeed;
            PDamping.SetDamping(1);
            StartCoroutine(WallClimbPushWait());
        }
        else
        {
            PDamping.ResetDamping();
        }
    }
    private IEnumerator WallClimbPushWait()
    {
        WallClimbPushing = true;
        float timeLeft = wallPushDuration;
        while (timeLeft > 0)
        {
            if (PDive.Diving)
            {
                WallClimbPushing = false;
                yield break;
            }
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        PDamping.ResetDamping();
        WallClimbPushing = false;
    }
}
