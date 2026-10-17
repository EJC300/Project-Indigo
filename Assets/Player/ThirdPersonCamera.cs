
using UnityEngine;


namespace Player
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Vector3 cameraOffset;

        private Vector3 lookAxis;
        private void LateUpdate()
        {
            Quaternion lookRotation = transform.rotation * Quaternion.Euler(lookAxis.x, lookAxis.y, 0.0f);

            Vector3 lookDirection = lookRotation * Vector3.forward * 10;

            Vector3 direction =  transform.position - lookDirection;

            mainCamera.transform.SetPositionAndRotation(direction,lookRotation);
        }
        public void LookCamera(Vector3 lookAxis)
        {
            this.lookAxis += lookAxis * Time.fixedDeltaTime * 8f;

            
            


        }

    }
}
