using System;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class AmbienceAudio : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private Vector3 size, center;
    [SerializeField] private float fadeSize;
    private float startVol;
    private Transform playerTransform;

    private void Start()
    {
        startVol = source.volume;
    }

    private void Update()
    {
        if (!playerTransform && NetworkManager.Singleton.LocalClient.PlayerObject != null)
            playerTransform = NetworkManager.Singleton.LocalClient.PlayerObject.transform;
        
        if (playerTransform == null) return;
        Vector3 localPos = playerTransform.position - transform.position - center;
        
        source.volume = 0;
        
        if (Mathf.Abs(localPos.x) > size.x / 2) return;
        if (Mathf.Abs(localPos.y) > size.y / 2) return;
        if (Mathf.Abs(localPos.z) > size.z / 2) return;

        Vector3 fadeBoxSize = FadeBoxSize();
        if (Mathf.Abs(localPos.x) > fadeBoxSize.x / 2 ||
            Mathf.Abs(localPos.y) > fadeBoxSize.y / 2 ||
            Mathf.Abs(localPos.z) > fadeBoxSize.z / 2)
        {
            //In fade
            Vector3 inFadeBox = new(
                Mathf.Clamp(localPos.x, -fadeBoxSize.x/2, fadeBoxSize.x/2),
                Mathf.Clamp(localPos.y, -fadeBoxSize.y/2, fadeBoxSize.y/2),
                Mathf.Clamp(localPos.z, -fadeBoxSize.z/2, fadeBoxSize.z/2));
            float dist = Vector3.Distance(inFadeBox + transform.position + center, playerTransform.position);
            source.volume = (1 - Mathf.Clamp01(dist * 2 / fadeSize))*startVol;
        }
        else
        {
            //Inside
            source.volume = startVol;
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Selection.Contains(gameObject)) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + center,size);
        Gizmos.color = Color.red;
        
        Gizmos.DrawWireCube(transform.position + center,FadeBoxSize());
    }
    #endif
    private Vector3 FadeBoxSize() => new(Mathf.Max(size.x - fadeSize, 0), Mathf.Max(size.y - fadeSize, 0),
        Mathf.Max(size.z - fadeSize, 0));
}
