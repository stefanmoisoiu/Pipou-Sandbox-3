using Unity.Netcode;
using UnityEngine;

public class PNetCollider : NetworkBehaviour
{
    [SerializeField] private CapsuleCollider capsuleCollider;
    private NetworkVariable<bool> colliderEnabled = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> colliderHeight = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> colliderYCenter = new(writePerm: NetworkVariableWritePermission.Owner);

    private void Update()
    {
        if (IsOwner)
        {
            colliderEnabled.Value = capsuleCollider.enabled;
            colliderHeight.Value = capsuleCollider.height;
            colliderYCenter.Value = capsuleCollider.center.y;
        }
        else
        {
            capsuleCollider.enabled = colliderEnabled.Value;
            capsuleCollider.height = colliderHeight.Value;
            capsuleCollider.center = new(capsuleCollider.center.x,colliderYCenter.Value,capsuleCollider.center.z);
        }
    }
}
