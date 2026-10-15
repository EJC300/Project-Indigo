using UnityEngine;
namespace Aircraft {
    public class ControlSurfaceAnimatable : MonoBehaviour
    {




        public void RotateToTargetAngle(Vector3 axis,float input)
        {

          
            Quaternion target = Quaternion.Euler(axis * -input);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target,  Time.fixedDeltaTime * 15);
            
        }



    }
}
