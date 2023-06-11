using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class PlayerRagdoll : NetworkBehaviour
{
    [SerializeField] private GameObject[] playerMeshes;
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private RagdollLimb[] ragdollLimbs;
    

    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider col;
    

    private readonly NetworkVariable<bool> ragdollEnabled = new (writePerm: NetworkVariableWritePermission.Owner);
    public static Action<bool> onSetRagdoll;

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
        lst.Add(new RagdollLimb() {baseBone = baseBone, ragdollBone = ragdollBone});
        for(int i = 0; i < ragdollBone.childCount; i ++)
        {
            lst.AddRange(AssignChildrenBone(baseBone.GetChild(i),ragdollBone.GetChild(i)));
        }
        return lst;
    }
    private void Start()
    {
        AssignBones();
        if (!IsOwner) return;
        onSetRagdoll += delegate(bool value)
        {
            ragdollEnabled.Value = value;
            if(value) OnStartRagdoll();
            else OnStopRagdoll();
        };
    }
    public static bool Ragdolling { get; private set; }

    public static void SetRagdoll(bool value)
    {
        if (Ragdolling == value) return;
        Ragdolling = value;
        onSetRagdoll?.Invoke(value);
    }

    public static void SetTempRagdoll(float duration)
    {
        SetRagdoll(true);
        stopRagdollWaitTimer = duration;
    }

    private void OnStartRagdoll()
    {
        rb.freezeRotation = false;
        col.enabled = false;
    }
    private void OnStopRagdoll()
    {
        rb.freezeRotation = true;
        col.enabled = true;
        rb.MovePosition(rb.position + Vector3.up);
        rb.MoveRotation(Quaternion.identity);
    }
    private void Update()
    {
        if (IsOwner && Input.GetKeyDown(KeyCode.R)) SetRagdoll(!Ragdolling);
        if(ragdoll.activeSelf != ragdollEnabled.Value)
        {
            ragdoll.SetActive(ragdollEnabled.Value);
            SetRagdollLimbs();
            col.enabled = !ragdollEnabled.Value;
            foreach(GameObject mesh in playerMeshes) mesh.SetActive(!ragdollEnabled.Value);
        }

        if (IsOwner && stopRagdollWaitTimer > 0)
        {
            stopRagdollWaitTimer -= Time.deltaTime;
            if(stopRagdollWaitTimer <= 0) SetRagdoll(false);
        }
    }

    private void SetRagdollLimbs()
    {
        for (int i = 0; i < ragdollLimbs.Length; i++)
        {
            ragdollLimbs[i].ragdollBone.rotation = ragdollLimbs[i].baseBone.rotation;
            ragdollLimbs[i].ragdollBone.position = ragdollLimbs[i].baseBone.position;
        }
    }
    [Serializable]
    public class RagdollLimb
    {
        public Transform baseBone, ragdollBone;
    }
}
