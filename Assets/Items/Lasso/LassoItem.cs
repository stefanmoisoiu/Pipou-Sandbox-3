using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class LassoItem : NetworkBehaviour
{
    [SerializeField] private InputBind startLassoBind,releaseLassoBind;
    
    [SerializeField] private LineRenderer lassoLine;
    [SerializeField] private GameObject lassoPrefab;
    [SerializeField] private Transform throwPos, lassoHoldPos;
    [SerializeField] private float throwForce;

    private ulong defaultLassoHitTargetValue = 999;
    private NetworkVariable<ulong> lassoHitTarget = new(999);
    private LassoItem lassoedOwnerLassoItem;
    

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

        if (IsOwner && lassoedOwnerLassoItem != null && lassoedOwnerLassoItem.lassoHitTarget.Value != NetworkObjectId) ReleaseFromLassoVictimServerRpc();
    }

    public void Select()
    {
        HoldingLasso = true;
        startLassoBind.Bind();
        releaseLassoBind.Bind();
        // InputManager.onUse += ChargeLasso;
        // InputManager.onStopUse += ThrowLasso;
    }

    public void Deselect()
    {
        HoldingLasso = false;
        startLassoBind.UnBind();
        releaseLassoBind.UnBind();
        // InputManager.onUse -= ChargeLasso;
        // InputManager.onStopUse -= ThrowLasso;
        RetractLasso();
    }

    #region Charge and Throw
    public void ChargeLasso()
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

    public void ThrowLasso()
    {
        if (!HoldingLasso || !ChargingLasso) return;
        ChargingLasso = false;
        LassoThrown = true;
        CreateLassoObjectServerRpc(throwPos.position, throwPos.forward,
            new ServerRpcParams
                { Receive = new ServerRpcReceiveParams { SenderClientId = NetworkManager.Singleton.LocalClientId } });
        onThrow?.Invoke();
    }
    #endregion

    #region Lasso Object
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
    #endregion
    
    #region Check Hit By Lasso
    private void OnTriggerEnter(Collider col)
    {
        if (!IsOwner) return;
        CheckHitByLasso(col);
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
    private void CheckHitByLassoServerRpc(NetworkObjectReference lassoRef)
    {
        lassoedOwnerLassoItem = NetworkManager
            .SpawnManager
            .GetPlayerNetworkObject(
                NetworkManager
                    .SpawnManager
                    .SpawnedObjects[lassoRef.NetworkObjectId]
                    .OwnerClientId)
            .GetComponent<PReferences>().lassoItem;
        lassoedOwnerLassoItem.HitTargetOwnerServerRpc(NetworkObjectId);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void HitTargetOwnerServerRpc(ulong value)
    {
        lassoHitTarget.Value = value;
        DeleteLassoObjectServerRpc();
    }
    #endregion

    #region Release Lassoed
    [ServerRpc(RequireOwnership = false)]
    private void ReleaseFromLassoVictimServerRpc()
    {
        ReleaseFromLassoVictimClientRpc(
            new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new[] { OwnerClientId } } });
    }
    [ClientRpc] private void ReleaseFromLassoVictimClientRpc(ClientRpcParams rpcParams) => ReleaseFromLassoVictim();
    private void ReleaseFromLassoVictim()
    {
        if (!Lassoed) return;
        lassoedOwnerLassoItem = null;
        Lassoed = false;
        PCamera.SetCurrentCam(PCamera.CamType.FPSCam);
        onStopLassoed?.Invoke();
        Debug.Log("Released from lasso");
    }
    
    
    [ServerRpc]
    private void ReleaseFromLassoOwnerServerRpc() =>
        lassoHitTarget.Value = defaultLassoHitTargetValue;
    #endregion

    #region Retract Lasso
    private void RetractLasso()
    {
        DeleteLassoObjectServerRpc();
        if (lassoHitTarget.Value != defaultLassoHitTargetValue)
            NetworkManager
                .SpawnManager
                .SpawnedObjects[lassoHitTarget.Value]
                .GetComponent<PReferences>()
                .lassoItem
                .ReleaseFromLassoVictimServerRpc();
        ReleaseFromLassoOwnerServerRpc();
        if (!LassoThrown) return;
        LassoThrown = false;
        onRetract?.Invoke();
    }
    #endregion
}
