using UnityEngine;
namespace Utilities
{
    public static class MathUtilities
    {
        public static float UnclampedInverseLerp(float a,float b, float value)
        {
            if (a == b) return 0.0f;

            float output = (value - a) / (b - a);

            return Mathf.Clamp(output, -1.0f, 1.0f);


        }
    }
}