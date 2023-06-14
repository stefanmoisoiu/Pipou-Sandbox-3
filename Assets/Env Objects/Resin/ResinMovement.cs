using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ResinMovement : NetworkBehaviour
{
    [SerializeField] private float rayLength;
    [SerializeField] private int rayCount;
    [SerializeField] private float x;
    [SerializeField] private LayerMask whatIsResin;
    [SerializeField] private Rigidbody rb;
    // [SerializeField] private Transform orientation;
    
    public static bool OnResin { get; private set; }
    private void Update()
    {
        if (!IsOwner) return;
        Vector3 normal = GetTargetNormal(out bool resinFound);
        OnResin = resinFound;
        if (resinFound && !PlayerRagdoll.Ragdolling && !PlayerJump.Jumping)
        {
            rb.transform.up = normal;
        }
        else
        {
            rb.transform.up = Vector3.up;
        }
    }

    private Vector3 GetTargetNormal(out bool resinFound)
    {
        float start = 1f / (rayCount - 1) - 1;
        float increment = (2 - 2f / (rayCount - 1)) / (rayCount - 1);
        resinFound = false;
        Vector3 targetNormal = Vector3.zero;
        for (int i = 0; i < rayCount; i++)
        {
            float s = start + i * increment;
            Vector3 coordinates =
                SphericalCoordinate(s * x, Mathf.PI / 2f * Mathf.Sign(s) * (1 - Mathf.Sqrt(1 - Mathf.Abs(s))));
            if (!Physics.Raycast(rb.transform.position + rb.transform.up * 0.1f, rb.transform.position + rb.transform.up * 0.1f + coordinates, out RaycastHit hit, rayLength,
                    whatIsResin)) continue;
            resinFound = true;
            targetNormal += hit.normal;
        }
        Debug.DrawRay(rb.transform.position,targetNormal,Color.magenta);
        return targetNormal.normalized;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        float start = 1f / (rayCount - 1) - 1;
        float increment = (2 - 2f / (rayCount - 1)) / (rayCount - 1);
        for (int i = 0; i < rayCount; i++)
        {
            float s = start + i * increment;
            Vector3 coordinates =
                SphericalCoordinate(s * x, Mathf.PI / 2f * Mathf.Sign(s) * (1 - Mathf.Sqrt(1 - Mathf.Abs(s))));
            Gizmos.DrawSphere(rb.transform.position + rb.transform.up * 0.1f + coordinates * rayLength,0.025f);
        }
        // int halfCircumferenceVertCount = Mathf.Floor((circumference / 2.0f) / distanceBetweenVerts);
        // for (int i = 0; i < rayCount; i++)
        // {
        //     currentRingDist += pointDist;
        //     float currentSectorDist = 0;
        //     for (int j = 0; j < rayCount; j++)
        //     {
        //         currentSectorDist += pointDist;
        //         float y = Mathf.Sin(currentRingDist);
        //         float x = Mathf.Cos(currentRingDist) * Mathf.Sin(currentRingDist);
        //         float z = Mathf.Sin(currentRingDist) * Mathf.Cos(currentRingDist);
        //         Gizmos.DrawLine(feetPos.position,feetPos.position + new Vector3(x,y,z)*rayLength);
        //     }
        // }
    }

    private Vector3 SphericalCoordinate(float x, float y)
    {
        return new Vector3(
            Mathf.Cos(x) * Mathf.Cos(y),
            Mathf.Sin(x) * Mathf.Cos(y),
            Mathf.Sin(y)
        );
    }
}
