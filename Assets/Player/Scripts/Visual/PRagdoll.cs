using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// [ExecuteAlways]
public class PRagdoll : NetworkBehaviour
{
    [SerializeField] private float testForce;
    
    [SerializeField] private GameObject[] playerMeshes;
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private RagdollLimb[] ragdollLimbs;
    [SerializeField] private Transform hip;
    


    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider col;
    

    public static Action<bool> onSetRagdoll;
    public static Action<Vector3> onStartRagdoll;
    public static Action onStopRagdoll;

    [Space]
    [SerializeField] private Transform baseArmature;
    [SerializeField] private Transform ragdollArmature;

    private static float stopRagdollWaitTimer;
    private void AssignBones()
    {
        ragdollLimbs = AssignChildrenBone(baseArmature, ragdollArmature).ToArray();
    }

    private List<RagdollLimb> AssignChildrenBone(Transform baseBone, Transform ragdollBone)
    {
        List<RagdollLimb> lst = new();
        if (ragdollBone.TryGetComponent(out Rigidbody ragdollRb))
            lst.Add(new RagdollLimb {baseBone = baseBone, ragdollBone = ragdollBone, ragdollRb = ragdollRb});
        for(int i = 0; i < ragdollBone.childCount; i ++)
        {
            lst.AddRange(AssignChildrenBone(baseBone.GetChild(i),ragdollBone.GetChild(i)));
        }
        return lst;
    }
    private void Start()
    {
        // AssignBones();
        if (!IsOwner) return;
        onStartRagdoll += delegate(Vector3 force)
        {
            OnStartRagdoll(force);
            OnStartRagdollServerRpc(force);
        };
        onStopRagdoll += delegate
        {
            OnStopRagdoll();
            OnStopRagdollServerRpc();
        };
    }
    public static bool Ragdolling { get; private set; }

    public static void StartRagdoll(Vector3 force)
    {
        if (Ragdolling) return;
        Ragdolling = true;
        onStartRagdoll?.Invoke(force);
        onSetRagdoll?.Invoke(true);
    }
    public static void StopRagdoll()
    {
        if (!Ragdolling) return;
        Ragdolling = false;
        onStopRagdoll?.Invoke();
        onSetRagdoll?.Invoke(false);
    }

    public static void SetTempRagdoll(float duration,Vector3 force)
    {
        StartRagdoll(force);
        stopRagdollWaitTimer = duration;
    }

    [ServerRpc] private void OnStartRagdollServerRpc(Vector3 force) => OnStartRagdollClientRpc(force);
    [ClientRpc] private void OnStartRagdollClientRpc(Vector3 force) {if(!IsOwner) OnStartRagdoll(force);}
    private void OnStartRagdoll(Vector3 force)
    {
        ragdoll.SetActive(true);
        SetRagdollLimbs(force);
        foreach(GameObject mesh in playerMeshes) mesh.SetActive(false);
        rb.isKinematic = true;
        col.enabled = false;
        if (IsOwner) PCamera.SetCurrentCam(PCamera.CamType.TPSCam);
    }
    
    [ServerRpc] private void OnStopRagdollServerRpc() => OnStopRagdollClientRpc();
    [ClientRpc] private void OnStopRagdollClientRpc() {if (!IsOwner) OnStopRagdoll();}
    private void OnStopRagdoll()
    {
        col.enabled = true;
        rb.isKinematic = false;
        foreach(GameObject mesh in playerMeshes) mesh.SetActive(true);
        rb.MoveRotation(Quaternion.identity);
        rb.MovePosition(hip.position + Vector3.up);
        hip.localPosition = Vector3.zero;
        ragdoll.SetActive(false);
        if (IsOwner) PCamera.SetCurrentCam(PCamera.CamType.FPSCam);
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Ragdolling) StopRagdoll();
            else StartRagdoll(Vector3.up * testForce);
        }

        if (stopRagdollWaitTimer > 0)
        {
            stopRagdollWaitTimer -= Time.deltaTime;
            if(stopRagdollWaitTimer <= 0) StopRagdoll();
        }
    }

    private void SetRagdollLimbs(Vector3 force)
    {
        for (int i = 0; i < ragdollLimbs.Length; i++)
        {
            if(ragdollLimbs[i].ragdollRb != null)
            {
                ragdollLimbs[i].ragdollRb.position = ragdollLimbs[i].baseBone.position;
                ragdollLimbs[i].ragdollRb.rotation = ragdollLimbs[i].baseBone.rotation;
                ragdollLimbs[i].ragdollRb.angularVelocity = Vector3.zero;
                ragdollLimbs[i].ragdollRb.velocity = force;
            }
            else
                ragdollLimbs[i].ragdollBone.SetPositionAndRotation(ragdollLimbs[i].baseBone.position,ragdollLimbs[i].baseBone.rotation);
        }
    }
    [Serializable]
    public class RagdollLimb
    {
        public Transform baseBone, ragdollBone;
        public Rigidbody ragdollRb;
    }
}
