using UnityEngine;

namespace Utilities
{

    [System.Serializable]

    public class SimplePID
    {
        [Header("Gains")]
        public float proportionalGain;
        public float integralGain;
        public float derivativeGain;

        [Header("Limits")]
        public float maxProportional;
        public float maxIntegral;
        public float maxDerivative;
        public float maxTotalOutput = 1.0f; // Added to cleanly normalize back to [-1, 1]


        private float errorIntegralAccumulator;
        private float previousError;
        private bool isFirstFrame = true;


        public float CalculatePID(float currentValue, float targetValue, float dt)
        {
            if (dt <= 0f) return 0f;

     
            float error = targetValue - currentValue;
            float p = error * proportionalGain;
            p = Mathf.Clamp(p, -maxProportional, maxProportional);

       
            errorIntegralAccumulator += error * dt;
            float maxIntegralValue = integralGain > 0 ? (maxIntegral / integralGain) : 0f;
            errorIntegralAccumulator = Mathf.Clamp(errorIntegralAccumulator, -maxIntegralValue, maxIntegralValue);
            float i = errorIntegralAccumulator * integralGain;

            float d = 0f;
            if (!isFirstFrame)
            {
                float errorDelta = (previousError - error) / dt;
                d = errorDelta * derivativeGain;
                d = Mathf.Clamp(d, -maxDerivative, maxDerivative);
            }
            else
            {
                isFirstFrame = false;
            }
            previousError = error;

            // 4. Total and Normalize
            float totalOutput = p + i + d;
            return Mathf.Clamp(totalOutput, -1, 1);
        }

        /// <summary>
        /// Resets the persistent states. Call this when a new missile spawns/enables.
        /// </summary>
        public void Reset()
        {
            errorIntegralAccumulator = 0f;
            previousError = 0f;
            isFirstFrame = true;
        }
    }

}