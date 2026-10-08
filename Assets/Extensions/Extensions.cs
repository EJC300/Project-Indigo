using UnityEngine;
namespace Utilities
{
    public static class Extensions
    {
        public static float GetAcceleration(this Rigidbody rb,ref Vector3 previousVelocity, ref Vector3 currentVelocity)
        {
            return (currentVelocity - previousVelocity).magnitude / Time.fixedDeltaTime;
        }
    }
}