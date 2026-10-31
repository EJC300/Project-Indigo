using System.Collections.Generic;
using UnityEngine;
namespace GameInfrastructure
{
    public class OriginShift : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float limit;
        [SerializeField] private float maxSpeed;
        public float Limit { get { return limit; } } 
        public static OriginShift instance;
        private List<Transform> targets = new List<Transform>();
        private float distance;
        void ShiftOriginObject(Transform trn)
        {
            distance = playerCamera.position.magnitude;
            if(distance > limit)
            {
                trn.position -= playerCamera.position;
            }
        }
        Vector3 ShiftOriginObject(Vector3 trn)
        {
            distance = playerCamera.position.magnitude;
            if (distance > limit)
            {
                trn -= playerCamera.position;
                return trn;
            }
            return trn;
        }
        public void AddToShifter(Transform trn)
        {
            targets?.Add(trn);
        }
        public void RemoveFromShifter(Transform trn)
        {
            targets?.Remove(trn);
        }
      
        void RunShifter()
        {
           
            for (int i = 0; i < targets.Count; i++) 
            {
                if (targets[i] != null)
                {
                    GameObject targetToShift = targets[i].gameObject;

                    ShiftOriginObject(targetToShift.transform);
                    Rigidbody rigidBody = targetToShift.GetComponentInChildren<Rigidbody>();
                    Collider collider = rigidBody.GetComponentInChildren<Collider>();
                    if (rigidBody != null)
                    {
                        rigidBody.maxLinearVelocity = maxSpeed;
                        rigidBody.maxAngularVelocity = maxSpeed;
                     
                  
                        rigidBody.MovePosition(ShiftOriginObject(rigidBody.transform.position));
                    }

                    //Is VFX Graphics later
                    ParticleSystem particles = targets[i].GetComponentInChildren<ParticleSystem>();


                    if (particles != null)
                    {

                        particles.gameObject.SetActive(!particles.gameObject.activeInHierarchy);
                        ShiftOriginObject(particles.transform);
                    }
                    TrailRenderer trails = targets[i].GetComponentInChildren<TrailRenderer>();
                    if (trails != null)
                    {
                        for (int j = 0; j < trails.positionCount; j++)
                        {


                            trails.SetPosition(j, ShiftOriginObject(trails.GetPosition(j)));
                        }
                    }
                }
            }
        }



        private void Start()
        {
            instance = this;
        }
        private void FixedUpdate()
        {
            RunShifter();
        }


    }
}