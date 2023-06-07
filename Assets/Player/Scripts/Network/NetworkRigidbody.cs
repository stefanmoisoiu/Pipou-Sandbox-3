using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkRigidbody : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    private NetworkVariable<bool> isKinematic = new(writePerm:NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> useGravity = new(writePerm:NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> freezeRotation = new(writePerm:NetworkVariableWritePermission.Owner);
    private void Update()
    {
        if (IsOwner)
        {
            isKinematic.Value = rb.isKinematic;
            useGravity.Value = rb.useGravity;
            freezeRotation.Value = rb.freezeRotation;
        }
        else
        {
            rb.isKinematic = isKinematic.Value;
            rb.useGravity = useGravity.Value;
            rb.freezeRotation = freezeRotation.Value;
        }
    }
}