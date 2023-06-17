using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class BouncePlayer : MonoBehaviour
{
    [SerializeField] private Vector3 force;
    [SerializeField] private UnityEvent uOnBounce;
    public static Action onBounce;
    [SerializeField] private int gizmoPointCount;
    [SerializeField] private float gizmoPointInterval = 0.5f;
    [SerializeField] private Color startGizmoColor = Color.red,endGizmoColor = Color.green;
    private Coroutine keepApplyingVelocityCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkObject networkObject) && networkObject.IsOwner &&
            other.TryGetComponent(out Rigidbody rb))
        {
            Vector3 vel = transform.rotation * force;
            
            rb.velocity = vel;
            if(keepApplyingVelocityCoroutine != null) StopCoroutine(keepApplyingVelocityCoroutine);
            keepApplyingVelocityCoroutine = StartCoroutine(KeepApplyingVelocity(rb,vel));
            
            uOnBounce?.Invoke();
            onBounce?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        float mass = 1f;
        List<Vector3> points = TrajectoryCalculator.GetTrajectoryPoints(
            transform.position,
            (transform.rotation * force).normalized,
            force.magnitude,
            mass,
            gizmoPointCount,
            gizmoPointInterval);
        for (int i = 0; i < points.Count; i++)
        {

            Gizmos.color = Color.Lerp(startGizmoColor, endGizmoColor, i / (float)points.Count);
            Gizmos.DrawSphere(points[i],0.3f);
        }
    }

    private IEnumerator KeepApplyingVelocity(Rigidbody rb,Vector3 vel)
    {
        float duration = 0.1f;
        float currentDuration = 0f;
        while (currentDuration < duration)
        {
            currentDuration += Time.deltaTime;
            rb.velocity = vel;
            yield return null;
        }
    }
}
