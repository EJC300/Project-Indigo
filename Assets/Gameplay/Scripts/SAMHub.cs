using System.Collections.Generic;
using UnityEngine;
namespace Gameplay
{
    public class SAMHub : MonoBehaviour
    {

        [SerializeField] private float fireRate;

        [SerializeField] private float loseAltitude = 6000;

        [SerializeField] private float minimumRange = 2000;
        [SerializeField] List<MissileLauncher> missileLaunchers = new List<MissileLauncher>();
        [SerializeField] private Rigidbody target;

        private Vector3 distanceToTarget;

        private Vector3 verticalRange;

        private float nextFire;

        public SAMHub samHub;
        private void Update()
        {
            CalculateDistances();
            FireMissiles();
        }
        bool CanFire()
        {
            if (( nextFire > Time.time))
            {
                nextFire = Time.time + fireRate;

                return  true;
            }
            else
            {
                return false;
            }
        }
        bool CheckCanFireInRange()
        {
            return distanceToTarget.magnitude > minimumRange && verticalRange.magnitude > loseAltitude;
        }
        void FireMissiles()
        {

            for (int i = 0; i < missileLaunchers.Count; ++i)
            {
                if (missileLaunchers[i].target != target)
                {
                    missileLaunchers[i].target = target;

                }




            }


            if ((Time.time > nextFire))
            {
                nextFire = Time.time + fireRate;
                for (int i = 0; i < missileLaunchers.Count; ++i)
                {

                    Debug.Log(CanFire());

                    if (CheckCanFireInRange())
                    {

                     
                        missileLaunchers[i].FireMissile();

                    }
                }


            }
        }
        
        void CalculateDistances()
        {
            distanceToTarget = target.position - transform.position;

            verticalRange = Vector3.Project(distanceToTarget,Vector3.up);
        }
        public void AddMissileLauncher(MissileLauncher launcher)
        {
            if (!missileLaunchers.Contains(launcher))
            {
                missileLaunchers.Add(launcher);
            }
        }
        public void RemoveMissileLauncher(MissileLauncher launcher)
        {
            if (missileLaunchers.Contains(launcher))
            {
                missileLaunchers.Remove(launcher);
            }
        }

    }
}
