using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class BouncePlayer : MonoBehaviour
{
    [SerializeField] private Vector3 force;
    [SerializeField] private UnityEvent onBounce;
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
            
            PlayerJump.SetJumping(true);
            
            onBounce?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        float mass = 1f;
        Vector3 vel = transform.rotation * force / mass;
        Vector3 launchPos = transform.position;
        for (int i = 0; i < gizmoPointCount; i++)
        {
            float timePassed = i * gizmoPointInterval;
            Vector3 calculatedPosition = launchPos + vel * timePassed;
            calculatedPosition.y =
                launchPos.y + vel.y * timePassed + Physics.gravity.y / 2f * timePassed * timePassed;

            Gizmos.color = Color.Lerp(startGizmoColor, endGizmoColor, i / (float)gizmoPointCount);
            Gizmos.DrawSphere(calculatedPosition,0.3f);
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
