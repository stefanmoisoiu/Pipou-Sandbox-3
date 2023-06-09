using UnityEngine;

public static class PlayerMovementCalculator
{
    public static Vector3 GetDir(Transform orientation) => InputManager.MoveInput.y * orientation.forward + InputManager.MoveInput.x * orientation.right;

    public static Vector3 GetGroundProjectedVector(Vector3 vector)
    {
        if(PlayerGrounded.IsGrounded) return Vector3.ProjectOnPlane(vector, PlayerGrounded.GroundHitRaycast.normal);
        return vector;
    }
    public static Vector3 GetVelocityProjectedVector(Vector3 velocity,Vector3 vector) =>
        Vector3.ProjectOnPlane(vector, velocity);

    public static Vector3 GetFullMovementDir(Vector3 dir, Vector3 velocity,float maxControllableSpeed)
    {
        Vector3 hNormal = new (PlayerGrounded.GroundHitRaycast.normal.x,0,PlayerGrounded.GroundHitRaycast.normal.z);
        if (PlayerGrounded.IsGrounded &&
            !PlayerGrounded.IsOnControllableSlope &&
            !PlayerSlideCalculator.FacingDownwards(dir,hNormal,.2f))
        {
            return Vector3.zero;
        }
        
        Vector3 result = dir;
        result = GetGroundProjectedVector(result);
        if (velocity.magnitude >= maxControllableSpeed && Vector3.Dot(dir, velocity.normalized) >= 0)
        {
            result = GetVelocityProjectedVector(velocity, result);
        }
        
        return result;
    }
    public static bool CanControlMovement(Vector3 normal,float maxAngle)
    {
        return Vector3.Angle(Vector3.up, normal) < maxAngle;
    }
}