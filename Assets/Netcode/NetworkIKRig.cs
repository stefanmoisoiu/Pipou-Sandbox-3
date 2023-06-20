using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class NetworkIKRig : NetworkBehaviour
{
    [SerializeField] private Rig rig;
    private NetworkVariable<float> weight = new (writePerm: NetworkVariableWritePermission.Owner);

    [SerializeField] private IKTarget[] ikTargets;
    [SerializeField] private float ikTargetLerpSpeed = 10;
    private void Update()
    {
        if (rig != null)
        {
            if (IsOwner) weight.Value = rig.weight;
            else rig.weight = weight.Value;
        }

        if (ikTargets != null && ikTargets.Length > 0)
        {
            foreach (IKTarget ikTarget in ikTargets)
            {
                if (IsOwner) ikTarget.localPos.Value = ikTarget.target.localPosition;
                else ikTarget.target.localPosition = Vector3.Lerp(ikTarget.target.localPosition,ikTarget.localPos.Value,ikTargetLerpSpeed*Time.deltaTime);
            }
        }
    }
    [Serializable]
    public class IKTarget
    {
        public Transform target;
        [HideInInspector]public NetworkVariable<Vector3> localPos = new(writePerm: NetworkVariableWritePermission.Owner);
    }
}
