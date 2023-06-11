using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BoxingGlovesItem : MonoBehaviour
{
    [SerializeField] private float punchForce,punchCooldown;
    [SerializeField] private Transform punchCheckPos;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float punchCheckSize;
    
    private bool canPunch = true;
    
    public void Select()
    {
        InputManager.onUse += Punch;
    }

    public void Deselect()
    {
        InputManager.onUse -= Punch;
    }

    private void Punch()
    {
        if (!canPunch) return;
        Collider[] cols = Physics.OverlapSphere(punchCheckPos.position, punchCheckSize, playerLayer);
        IEnumerable<Collider> playerCols = cols.Where(IsOtherPlayerCol);
        foreach (Collider playerCol in playerCols)
        {
            
        }
        canPunch = false;
        Invoke(nameof(ResetPunch),punchCooldown);
    }

    private bool IsOtherPlayerCol(Collider col)
    {
        if (!col.TryGetComponent(out NetworkObject obj) || obj.IsOwner) return false;
        return true;
    }
    private void ResetPunch() => canPunch = true;
}
