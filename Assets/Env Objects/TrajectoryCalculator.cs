using System.Collections.Generic;
using UnityEngine;

public static class TrajectoryCalculator
{
        public static List<Vector3> GetTrajectoryPoints(Vector3 startPos, Vector3 startDir, float startForce,
                float mass, int pointCount,
                float pointTimeInterval)
        {
                Vector3 startVel = startDir * startForce / mass;
                List<Vector3> points = new();
                for (int i = 0; i < pointCount; i++)
                {
                        float timePassed = i * pointTimeInterval;
                        Vector3 calculatedPosition = startPos + startVel * timePassed;
                        calculatedPosition.y =
                                startPos.y + startVel.y * timePassed + Physics.gravity.y / 2f * timePassed * timePassed;

                        points.Add(calculatedPosition);
                }

                return points;
        }

        public static float DurationToPointInterval(int pointCount, float duration) => duration / pointCount;
}