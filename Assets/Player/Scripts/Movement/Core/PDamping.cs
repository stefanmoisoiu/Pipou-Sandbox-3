using Unity.Netcode;
using UnityEngine;

public class PDamping : NetworkBehaviour
{
        [SerializeField] [Range(0, 1)] private float groundNormalDamping = 0.875f,groundTooFastDamping = 0.935f;

        [SerializeField] private AnimationCurve dampLerpEase;
        
        [SerializeField]private Rigidbody rb;
        
        private static float _damping;
        private static float _startLerpDamping;
        private static float _dampLerpAdvancement = 1;
        private static float _dampLerpDuration = 1;
        private static bool _dampingLocked;

        private void Start()
        {
                if (!IsOwner) return;
                _damping = groundNormalDamping;
                PRagdoll.onSetRagdoll += delegate(bool value) { if(value) ResetDamping(); };
        }
        
        private void Update()
        {
                if (_dampLerpAdvancement < 1) _dampLerpAdvancement += Time.deltaTime / _dampLerpDuration;
        }

        private bool CanApplyDamping() => PGrounded.IsGrounded &&
                                          PJump.CanJump &&
                                          !PJump.Jumping &&
                                          !PRagdoll.Ragdolling;
        private void FixedUpdate()
        {
                if (!IsOwner) return;
                if (!CanApplyDamping()) return;

                if (!_dampingLocked)
                {
                        float hVel = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
                        
                        float targetDamping = PMovement.TooFast ? groundTooFastDamping : groundNormalDamping;
                        targetDamping = PGrounded.IsOnControllableSlope ? targetDamping : 1;
                        if (_dampLerpAdvancement < 1)
                        {
                                _damping = Mathf.Lerp(_startLerpDamping, targetDamping,
                                        dampLerpEase.Evaluate(_dampLerpAdvancement));
                        }
                        else _damping = targetDamping;   
                }
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y,rb.velocity.z) * _damping;
        }

        public static void SetDamping(float value)
        {
                _damping = value;
                _dampLerpAdvancement = 1;
                _dampingLocked = true;
        }

        public static void ResetDamping()
        {
                _dampLerpAdvancement = 1;
                _dampingLocked = false;
        }
        /// <summary>
        /// Temporarily sets the damping to a value, then lerps back to base damping.
        /// Locks the base damping from being used, to instead take the interpolated damping value.
        /// </summary>
        /// <param name="value">new damping value</param>
        /// <param name="lerpDuration">duration of transition</param>
        public static void SetTempDamping(float value,float lerpDuration)
        {
                _startLerpDamping = value;
                _damping = value;
                _dampLerpDuration = lerpDuration;
                _dampLerpAdvancement = 0;
                _dampingLocked = false;
        }
}