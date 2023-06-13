using System;
using UnityEngine;

public static class PlayerSlideCalculator
{
        public static void ApplySlideDamping(Vector3 velocity,SlideDampingProperties s) =>
                ApplySlideDamping(
                        velocity,
                        s.minVelDotValue,
                        s.minSlideAngle, s.maxSlideAngle,
                        s.minSlideDamping, s.maxSlideDamping);
        public static void ApplySlideDamping(
                Vector3 velocity, float minVelDotValue,
                float minSlideAngle,float maxSlideAngle,
                float minSlideDamping, float maxSlideDamping)
        {
                if (!PGrounded.IsGrounded)
                {
                        PDamping.SetDamping(minSlideDamping);
                        return;
                }

                if (!FacingDownwards(velocity,PGrounded.GroundHitRaycast.normal,minVelDotValue))
                {
                        PDamping.SetDamping(minSlideDamping);
                        return;
                }
                
                float groundAngle = Vector3.Angle(PGrounded.GroundHitRaycast.normal, Vector3.up);
                groundAngle -= minSlideAngle;
                groundAngle = Mathf.Max(groundAngle, 0);
                
                float angleAdvancement = Mathf.Clamp01(groundAngle / (maxSlideAngle - minSlideAngle));
                float targetDamping = Mathf.Lerp(minSlideDamping, maxSlideDamping, angleAdvancement);
                
                
                PDamping.SetDamping(targetDamping);
        }

        public static bool FacingDownwards(Vector3 velocity, Vector3 normal, float minDotValue)
        {
                Vector3 hVelDir = new Vector3(velocity.x, 0, velocity.z).normalized;
                Vector3 hNormalDir = new Vector3(normal.x, 0, normal.z).normalized;
                return Vector3.Dot(hVelDir, hNormalDir) > minDotValue;
        }
        [Serializable]
        public struct SlideDampingProperties
        {
                public float minVelDotValue;
                public float minSlideAngle, maxSlideAngle;
                public float minSlideDamping, maxSlideDamping;
        }
}