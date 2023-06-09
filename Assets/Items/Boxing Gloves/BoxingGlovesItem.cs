using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class BoxingGlovesItem : NetworkBehaviour
{
    [SerializeField] private InputBind punchBind;
    
    [SerializeField] private float punchForce,punchRagdollDuration,punchCooldown;
    [SerializeField] private Transform punchCheckPos;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float punchCheckSize;
    [SerializeField] private Transform orientation;
    [SerializeField] private UnityEvent onPunch;
    
    private bool canPunch = true;
    
    public void Select()
    {
        punchBind.Bind();
    }

    public void Deselect()
    {
        punchBind.UnBind();
    }

    public void Punch()
    {
        if (!canPunch) return;
        onPunch?.Invoke();
        Collider[] cols = Physics.OverlapSphere(punchCheckPos.position, punchCheckSize, playerLayer);
        IEnumerable<Collider> playerCols = cols.Where(IsOtherPlayerCol);
        Vector3 force = (orientation.forward + Vector3.up).normalized * punchForce;
        List<ulong> clientIDs = new();
        foreach (Collider playerCol in playerCols)
        {
            clientIDs.Add(playerCol.GetComponent<NetworkObject>().OwnerClientId);
            
        }
        ApplyPunchServerRpc(force,clientIDs.ToArray());
        canPunch = false;
        Invoke(nameof(ResetPunch),punchCooldown);
    }

    [ServerRpc]
    private void ApplyPunchServerRpc(Vector3 force,ulong[] targetIds)
    {
        ApplyPunchClientRpc(force,new ClientRpcParams
        {
            Send = new ClientRpcSendParams{TargetClientIds = targetIds}
        });
    }
       
    [ClientRpc]
    private void ApplyPunchClientRpc(Vector3 force,ClientRpcParams rpcParams)
    {
        PRagdoll.SetTempRagdoll(punchRagdollDuration,force);
    }

    private bool IsOtherPlayerCol(Collider col)
    {
        if (!col.TryGetComponent(out NetworkObject obj) || obj.IsOwner) return false;
        return true;
    }
    private void ResetPunch() => canPunch = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(punchCheckPos.position,punchCheckSize);
    }
}
