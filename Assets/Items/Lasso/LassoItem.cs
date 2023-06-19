using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LassoItem : NetworkBehaviour
{
    [SerializeField] private LineRenderer lassoLine;
    [SerializeField] private GameObject lassoPrefab;
    [SerializeField] private Transform throwPos, lassoHoldPos;
    [SerializeField] private float throwForce;

    private ulong defaultLassoHitTargetValue = 999;
    private NetworkVariable<ulong> lassoHitTarget = new(999);
    

    private GameObject lassoInstance;

    public static bool HoldingLasso { get; private set; }
    public static bool ChargingLasso { get; private set; }
    public static bool LassoThrown { get; private set; }
    public static bool Lassoed { get; private set; }

    public UnityEvent onCharge, onThrow, onRetract, onLassoed, onStopLassoed;

    private void Update()
    {
        lassoLine.SetPosition(0, lassoHoldPos.position);
        if (lassoHitTarget.Value == defaultLassoHitTargetValue)
        {
            lassoLine.SetPosition(1,
                lassoInstance is null ? lassoHoldPos.position : lassoInstance.transform.GetChild(0).position);
        }
        else
        {
            lassoLine.SetPosition(1, NetworkManager.Singleton.SpawnManager.SpawnedObjects[lassoHitTarget.Value].transform.position);
        }
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
        RetractLasso();
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
        CreateLassoObjectServerRpc(throwPos.position, throwPos.forward,
            new ServerRpcParams
                { Receive = new ServerRpcReceiveParams { SenderClientId = NetworkManager.Singleton.LocalClientId } });
        onThrow?.Invoke();
    }

    [ServerRpc]
    private void CreateLassoObjectServerRpc(Vector3 pos, Vector3 throwDir, ServerRpcParams rpcParams)
    {
        Debug.Log($"pos : {pos} dir : {throwDir} from : {rpcParams.Receive.SenderClientId}");
        lassoInstance = Instantiate(lassoPrefab, pos, Quaternion.identity);
        lassoInstance.transform.forward = throwDir;
        NetworkObject lassoNetworkObject = lassoInstance.GetComponent<NetworkObject>();
        lassoNetworkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId, true);
        SetLassoObjectClientRpc(lassoNetworkObject, throwDir);
    }

    [ClientRpc]
    private void SetLassoObjectClientRpc(NetworkObjectReference reference, Vector3 throwDir)
    {
        lassoInstance = NetworkManager.Singleton.SpawnManager.SpawnedObjects[reference.NetworkObjectId].gameObject;
        lassoInstance.GetComponent<Rigidbody>().velocity = throwDir * throwForce;
    }

    [ServerRpc]
    private void DeleteLassoObjectServerRpc()
    {
        if (lassoInstance == null) return;
        lassoInstance.GetComponent<NetworkObject>().Despawn();
        Destroy(lassoInstance);
        lassoInstance = null;
        DeleteLassoObjectClientRpc();
    }
    [ClientRpc]
    private void DeleteLassoObjectClientRpc() =>
        lassoInstance = null;

    private void OnTriggerEnter(Collider col)
    {
        if (!IsOwner) return;
        CheckHitByLasso(col);
    }
    [ServerRpc(RequireOwnership = false)]
    private void HitTargetServerRpc(ulong value)
    {
        lassoHitTarget.Value = value;
        DeleteLassoObjectServerRpc();
    }
    public void CheckHitByLasso(Collider col)
    {
        if (!col.CompareTag("Lasso Instance") || Lassoed || !col.TryGetComponent(out NetworkObject lassoNetworkObject) || lassoNetworkObject.IsOwner) return;
        Lassoed = true;
        onLassoed?.Invoke();
        PCamera.SetCurrentCam(PCamera.CamType.TPSCam);
        CheckHitByLassoServerRpc(lassoNetworkObject);
        Debug.Log("Hit by lasso");
    }
    [ServerRpc]
    private void CheckHitByLassoServerRpc(NetworkObjectReference lassoRef) =>
        NetworkManager.SpawnManager.GetPlayerNetworkObject(
                NetworkManager.SpawnManager.SpawnedObjects[lassoRef.NetworkObjectId].OwnerClientId)
            .GetComponent<PReferences>().lassoItem.HitTargetServerRpc(NetworkObjectId);
    [ClientRpc]
    private void ReleaseFromLassoVictimClientRpc(ClientRpcParams rpcParams)
    {
        if (!Lassoed) return;
        Lassoed = false;
        PCamera.SetCurrentCam(PCamera.CamType.FPSCam);
        onStopLassoed?.Invoke();
        Debug.Log("Released from lasso");
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReleaseFromLassoVictimServerRpc()
    {
        ReleaseFromLassoVictimClientRpc(
            new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } } });
    }
    [ServerRpc]
    private void ReleaseFromLassoOwnerServerRpc() =>
        lassoHitTarget.Value = defaultLassoHitTargetValue;
    private void RetractLasso()
    {
        if (!LassoThrown) return;
        DeleteLassoObjectServerRpc();
        if (lassoHitTarget.Value != defaultLassoHitTargetValue)
            NetworkManager
                .SpawnManager
                .SpawnedObjects[lassoHitTarget.Value]
                .GetComponent<PReferences>()
                .lassoItem
                .ReleaseFromLassoVictimServerRpc();
        ReleaseFromLassoOwnerServerRpc();
        LassoThrown = false;
        onRetract?.Invoke();
    }
}
