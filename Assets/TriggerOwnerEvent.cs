using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOwnerEvent : MonoBehaviour
{
    public UnityEvent<Collider> onEnter, onStay, onExit;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out NetworkObject obj) || obj.IsOwner == false) return;
        onEnter?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out NetworkObject obj) || obj.IsOwner == false) return;
        onStay?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out NetworkObject obj) || obj.IsOwner == false) return;
        onExit?.Invoke(other);
    }
}
