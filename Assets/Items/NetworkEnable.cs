using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkEnable : NetworkBehaviour
{
        [SerializeField] private GameObject target;
        private NetworkVariable<bool> networkEnable = new(writePerm: NetworkVariableWritePermission.Owner);

        private void Update()
        {
                if (IsOwner) networkEnable.Value = target.activeSelf;
                else target.SetActive(networkEnable.Value);
        }
}