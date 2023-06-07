using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float holdDuration,useMatSpeed = 15,moveSpeed = 10,canUseCooldown = 1.5f,rotLerpSpeed = 5,endRotDuration = .6f;
    [FormerlySerializedAs("rotEaseCurve")] [SerializeField] private AnimationCurve moveEaseCurve;
    [SerializeField] private bool updatePathWhileUsing;
    
    [SerializeField] private Transform playerHoldPos;
    [SerializeField] private Renderer renderer;
    private float startMatRotSpeed;

    [SerializeField] private SuperSpeedCalculator.SuperSpeedProperties superSpeedSpline;

    private static Coroutine MovePlayerCoroutine;
    public static Action onStartUsing;
    public static bool SuperSpeedConveyor { get; private set; }
    private bool usingThisConveyor;
    private bool canUseConveyor = true;
    private static readonly int Speed = Shader.PropertyToID("_Speed");

    private void Start()
    {
        startMatRotSpeed = renderer.materials[1].GetFloat(Speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (usingThisConveyor || !canUseConveyor) return;
        if (other.CompareTag("Player") &&
            other.GetComponent<NetworkObject>().IsOwner &&
            other.TryGetComponent(out Rigidbody rb) &&
            other.TryGetComponent(out PlayerReferences references))
        {
            onStartUsing?.Invoke();
            if(MovePlayerCoroutine != null) StopCoroutine(MovePlayerCoroutine);
            StartCoroutine(HoldPlayer(rb,references));
        }
    }

    private IEnumerator HoldPlayer(Rigidbody rb,PlayerReferences references)
    {
        SuperSpeedConveyor = true;
        usingThisConveyor = true;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        renderer.materials[1].SetFloat(Speed,useMatSpeed);
        
        float duration = holdDuration;
        float currentDuration = 0;
        
        Vector3 startPos = rb.position;
        
        Quaternion startRot = rb.rotation;
        Quaternion startOrientationRot = references.orientation.localRotation;
        Quaternion startHeadRot = references.head.localRotation;
        
        PlayerCamera.SetCanLook(false);
        
        while (currentDuration < duration)
        {
            float lerp = moveEaseCurve.Evaluate(currentDuration / duration);
            rb.position = Vector3.Lerp(startPos, playerHoldPos.position,lerp);
            
            rb.transform.rotation = Quaternion.Lerp(startRot,transform.rotation,lerp);
            references.orientation.localRotation = Quaternion.Lerp(startOrientationRot, Quaternion.identity, lerp);
            references.head.localRotation = Quaternion.Lerp(startHeadRot, Quaternion.identity, lerp);
            
            currentDuration += Time.deltaTime;
            yield return null;
        }
        usingThisConveyor = false;
        renderer.materials[1].SetFloat(Speed,startMatRotSpeed);
        Invoke(nameof(ResetCanUse),canUseCooldown);
        MovePlayerCoroutine = StartCoroutine(MovePlayer(rb,references));
    }

    private IEnumerator MovePlayer(Rigidbody rb,PlayerReferences references)
    {
        Spline spline =
            SuperSpeedCalculator.SuperSpeedSpline(transform.position, transform.up, transform.forward,
                superSpeedSpline);
        float duration = superSpeedSpline.dist / moveSpeed;
        float currentDuration = 0;
        while (currentDuration < duration)
        {
            if (updatePathWhileUsing)
            {
                spline =
                    SuperSpeedCalculator.SuperSpeedSpline(transform.position, transform.up, transform.forward,
                        superSpeedSpline);
            }
            
            float advancement = currentDuration / duration;
            float lookAdvancement = Mathf.Clamp01((currentDuration + .5f) / duration);
            spline.Evaluate(advancement, out float3 pos, out float3 tangent, out float3 upVector);
            spline.Evaluate(lookAdvancement, out float3 lookPos, out float3 lookTangent, out float3 lookUpVector);
            rb.position = pos;
            Quaternion dirRot = Quaternion.LookRotation(lookPos - pos, upVector);
            rb.MoveRotation(Quaternion.Lerp(rb.rotation,dirRot,rotLerpSpeed * Time.deltaTime));
            currentDuration += Time.deltaTime;
            yield return null;
        }

        currentDuration = 0;
        duration = endRotDuration;
        Quaternion startRot = rb.rotation;

        SuperSpeedConveyor = false;
        rb.isKinematic = false;
        
        while (currentDuration < duration)
        {
            rb.rotation = Quaternion.Lerp(startRot,Quaternion.identity,moveEaseCurve.Evaluate(currentDuration / duration));
            currentDuration += Time.deltaTime;
            yield return null;
        }
        rb.rotation = Quaternion.identity;
        PlayerCamera.SetTargetRotation(Vector2.zero);
        PlayerCamera.SetCanLook(true);
    }

    private void ResetCanUse() => canUseConveyor = true;

    private void OnDrawGizmos()
    {
        Spline spline =
            SuperSpeedCalculator.SuperSpeedSpline(transform.position, transform.up, transform.forward,
                superSpeedSpline);
        for (int i = 1; i < spline.Knots.Count(); i++)
        {
            BezierKnot lastKnot = spline.Knots.ElementAt(i-1);
            BezierKnot nextKnot = spline.Knots.ElementAt(i);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lastKnot.Position,nextKnot.Position);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(nextKnot.Position, (Quaternion)nextKnot.Rotation * Vector3.up);
        }
    }
}
