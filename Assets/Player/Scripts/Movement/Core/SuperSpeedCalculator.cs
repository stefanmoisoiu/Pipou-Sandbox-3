using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public static class SuperSpeedCalculator
{
    public static Spline SuperSpeedSpline(Vector3 startPos, Vector3 startUp, Vector3 startForward,
        SuperSpeedProperties properties) => SuperSpeedSpline(
        startPos, startUp, startForward,
        properties.rayCheckLength, properties.yOffset, properties.distBtwRays,
        properties.dist, properties.sampleEveryXPoints,properties.mask);
    public static Spline SuperSpeedSpline(Vector3 startPos,Vector3 startUp,Vector3 startForward,
        float rayCheckLength,float yOffset,float distBtwRays,float dist,int sampleEveryXPoints,LayerMask mask)
    {
        Spline spline = new();
        List<BezierKnot> knots = new();
        Vector3 forward = startForward;
        Vector3 currentPos = startPos + startUp * yOffset;
        Vector3 currentNormal = startUp;
        int rayCount = Mathf.FloorToInt(dist / distBtwRays);
        for (int i = 0; i < rayCount; i++)
        {
            if (Physics.Raycast(currentPos, -currentNormal, out RaycastHit hit, rayCheckLength,mask))
            {
                forward = Quaternion.FromToRotation(currentNormal,hit.normal) * forward;
                currentNormal = hit.normal;
                currentPos = hit.point + hit.normal * yOffset;
                currentPos += forward * distBtwRays;
                if (i % sampleEveryXPoints != 0) continue;
                
                BezierKnot knot = new (currentPos)
                {
                    Rotation = Quaternion.LookRotation(forward, currentNormal)
                };
                knots.Add(knot);
            }
            else
            {
                spline.Knots = knots;
                return spline;
            }
        }
        spline.Knots = knots;
        return spline;
    }
    [Serializable]
    public struct SuperSpeedProperties
    {
        public float rayCheckLength, yOffset, distBtwRays, dist;
        public int sampleEveryXPoints;
        public LayerMask mask;
    }
}
