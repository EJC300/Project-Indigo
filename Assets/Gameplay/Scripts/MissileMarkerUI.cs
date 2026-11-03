using Gameplay;
using System.Collections.Generic;
using UnityEngine;
namespace GameUI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MissileMarkerUI : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private MissileMarker missileMarkerUIPrefab;
        [SerializeField] private int MaxUIIcons = 5;

        private readonly List<BasicMissile> missileMarks = new List<BasicMissile>();
        private readonly List<BasicMissile> sortBuffer = new List<BasicMissile>();
        private readonly List<MissileMarker> missileMarkers = new List<MissileMarker>();

        // Number of markers currently being shown (recomputed every frame)
        public int missileCount;

        private Comparison<BasicMissile> byDistanceToPlayer;

        public void RegisterMissile(BasicMissile missile)
        {
            if (!missileMarks.Contains(missile))
            {
                missileMarks.Add(missile);
            }
        }

        public void DeRegisterMissile(BasicMissile missile)
        {
            missileMarks.Remove(missile);
        }

        private void Awake()
        {
            // Cached once so sorting doesn't allocate every frame
            byDistanceToPlayer = (a, b) =>
            {
                Vector3 p = player.position;
                float da = (a.transform.position - p).sqrMagnitude;
                float db = (b.transform.position - p).sqrMagnitude;
                return da.CompareTo(db);
            };
        }

        private void Start()
        {
            RectTransform parent = GetComponent<RectTransform>();

            for (int i = 0; i < MaxUIIcons; i++)
            {
                MissileMarker marker = Instantiate(missileMarkerUIPrefab, parent);
                marker.gameObject.SetActive(false);
                missileMarkers.Add(marker);
            }
        }

        private void FixedUpdate()
        {
            // Drop any missiles that were destroyed without deregistering
            missileMarks.RemoveAll(m => m == null);

            // Work on a copy so we never reorder the master list
            sortBuffer.Clear();
            sortBuffer.AddRange(missileMarks);

            // Only need to sort when there are more missiles than icons
            if (sortBuffer.Count > missileMarkers.Count)
            {
                sortBuffer.Sort(byDistanceToPlayer);
            }

            missileCount = Mathf.Min(sortBuffer.Count, missileMarkers.Count);

            for (int i = 0; i < missileMarkers.Count; i++)
            {
                MissileMarker marker = missileMarkers[i];
                bool show = i < missileCount;

                if (marker.gameObject.activeSelf != show)
                {
                    marker.gameObject.SetActive(show);
                }

                if (show)
                {
                    marker.UpdatePositionAndLogic(sortBuffer[i].transform, player);
                }
            }
        }
    }
}