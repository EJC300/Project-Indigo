using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
namespace GameInfrastructure
{
  

    public class OriginShift : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private float limit;
        [SerializeField] private float maxSpeed;

        [Header("VFX Graph")]
        [Tooltip("Reinit VFX Graph effects after a shift. Only useful for World-space graphs. " +
                 "Leave off for one-shot effects (explosions etc.), since Reinit replays them.")]
        [SerializeField] private bool reinitVfxOnShift = false;

        public float Limit { get { return limit; } }
        public static OriginShift instance;

        // Components are looked up once when a target is registered,
        // instead of every FixedUpdate.
        private class ShiftEntry
        {
            public Transform root;
            public Rigidbody rigidBody;
            public VisualEffect[] vfx;
            public TrailRenderer[] trails;
        }

        private readonly List<ShiftEntry> targets = new List<ShiftEntry>();

        private void Awake()
        {
            // Awake (not Start) so other scripts can safely call OriginShift.instance in their own Start
            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }

        public void AddToShifter(Transform trn)
        {
            if (trn == null || IndexOf(trn) >= 0) return; // ignore nulls / duplicates (a duplicate would be shifted twice)

            targets.Add(new ShiftEntry
            {
                root = trn,
                rigidBody = trn.GetComponentInChildren<Rigidbody>(),
                vfx = trn.GetComponentsInChildren<VisualEffect>(),
                trails = trn.GetComponentsInChildren<TrailRenderer>()
            });
        }

        public void RemoveFromShifter(Transform trn)
        {
            int index = IndexOf(trn);
            if (index >= 0) targets.RemoveAt(index);
        }

        private int IndexOf(Transform trn)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                if (targets[i].root == trn) return i;
            }
            return -1;
        }

        private void FixedUpdate()
        {
            RunShifter();
        }

        private void RunShifter()
        {
            if (playerCamera == null) return;

            // Decide ONCE per step and use one offset for everything.
            // (Checking per object meant that once the camera had been shifted,
            // every object processed after it saw a distance of ~0 and was skipped.)
            Vector3 offset = playerCamera.position;
            bool shift = offset.sqrMagnitude > limit * limit;

            // Backwards so destroyed targets can be removed while looping
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                ShiftEntry entry = targets[i];

                if (entry.root == null)
                {
                    targets.RemoveAt(i);
                    continue;
                }

                if (entry.rigidBody != null)
                {
        
                    entry.rigidBody.maxLinearVelocity = maxSpeed;
                    entry.rigidBody.maxAngularVelocity = maxSpeed;
                }

                if (shift)
                {
                    Shift(entry, offset);
                }
            }

            if (shift)
            {
                // Make the physics engine pick up the moved transforms right away
                Physics.SyncTransforms();
            }
        }

        private void Shift(ShiftEntry entry, Vector3 offset)
        {

            entry.root.position -= offset;


            if (reinitVfxOnShift)
            {
                for (int v = 0; v < entry.vfx.Length; v++)
                {
                    if (entry.vfx[v] != null) entry.vfx[v].Reinit();
                }
            }

            // Trail points are stored in world space
            for (int t = 0; t < entry.trails.Length; t++)
            {
                TrailRenderer trail = entry.trails[t];
                if (trail == null) continue;

                for (int j = 0; j < trail.positionCount; j++)
                {
                    trail.SetPosition(j, trail.GetPosition(j) - offset);
                }
            }
        }
    }
}
