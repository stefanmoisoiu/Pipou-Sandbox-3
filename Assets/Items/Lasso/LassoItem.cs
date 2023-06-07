using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LassoItem : MonoBehaviour
{
    [SerializeField] private LineRenderer lassoLine;
    [SerializeField] private GameObject lassoPrefab;
    [SerializeField] private Transform throwPos,lassoHoldPos;
    [SerializeField] private float throwForce;
    
    private GameObject lassoInstance;
    
    public static bool HoldingLasso { get; private set; }
    public static bool ChargingLasso { get; private set; }
    public static bool LassoThrown { get; private set; }
    
    public UnityEvent onCharge, onThrow,onRetract;

    private void Update()
    {
        lassoLine.SetPosition(0,lassoHoldPos.position);
        lassoLine.SetPosition(1,
            lassoInstance is null ? lassoHoldPos.position : lassoInstance.transform.GetChild(0).position);
    }

    public void Select()
    {
        HoldingLasso = true;
        InputManager.onUse += ChargeLasso;
        InputManager.onStopUse += ThrowLasso;
    }

    public void Deselect()
    {
        HoldingLasso = false;
        InputManager.onUse -= ChargeLasso;
        InputManager.onStopUse -= ThrowLasso;
    }
    private void ChargeLasso()
    {
        if (!HoldingLasso || ChargingLasso) return;
        if (LassoThrown)
        {
            RetractLasso();
            return;
        }

        ChargingLasso = true;
        onCharge?.Invoke();
    }

    private void ThrowLasso()
    {
        if (!HoldingLasso || !ChargingLasso) return;
        ChargingLasso = false;
        LassoThrown = true;
        CreateLassoObjectServerRpc(throwPos.position + throwPos.forward, throwPos.forward,
            new ServerRpcParams(){Receive = new ServerRpcReceiveParams(){SenderClientId=NetworkManager.Singleton.LocalClientId}});
        onThrow?.Invoke();
    }

    [ServerRpc]
    private void CreateLassoObjectServerRpc(Vector3 pos,Vector3 dir,ServerRpcParams rpcParams)
    {
        lassoInstance = Instantiate(lassoPrefab,pos,Quaternion.identity);
        lassoInstance.transform.forward = dir;
        lassoInstance.GetComponent<Rigidbody>().velocity = dir * throwForce;
        NetworkObject lassoNetworkObject = lassoInstance.GetComponent<NetworkObject>();
        lassoNetworkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        SetLassoObjectClientRpc(lassoNetworkObject);
    }

    [ClientRpc]
    private void SetLassoObjectClientRpc(NetworkObjectReference reference)
    {
        lassoInstance = NetworkManager.Singleton.SpawnManager.SpawnedObjects[reference.NetworkObjectId].gameObject;
    }
    [ServerRpc]
    private void DeleteLassoObjectServerRpc()
    {
        lassoInstance.GetComponent<NetworkObject>().Despawn();
        Destroy(lassoInstance);
        DeleteLassoObjectClientRpc();
    }
    [ClientRpc]
    private void DeleteLassoObjectClientRpc()
    {
        lassoInstance = null;
    }
    private void RetractLasso()
    {
        DeleteLassoObjectServerRpc();
        LassoThrown = false;
        onRetract?.Invoke();
    }
}
