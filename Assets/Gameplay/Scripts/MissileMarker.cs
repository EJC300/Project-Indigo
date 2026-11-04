using UnityEngine;
using Utilities;
namespace Gameplay
{
    using UnityEngine;

    public class MissileMarker : MonoBehaviour
    {
        [SerializeField] private RectTransform missileMarker;

        [Header("Scale by distance")]
        [Tooltip("At or beyond this distance the marker uses minScale.")]
        [SerializeField] private float maxDistance = 500f;
        [SerializeField] private float minScale = 0.4f;
        [SerializeField] private float maxScale = 1.5f;

        [Header("Marker bounds (fraction of screen size, measured from center)")]
        [SerializeField, Range(0f, 0.5f)] private float horizontalLimit = 0.33f;
        [SerializeField, Range(0f, 0.5f)] private float verticalLimit = 0.33f;

        private Camera cam;

        public void UpdatePositionAndLogic(Transform missile, Transform player)
        {
          
            if (cam == null) cam = Camera.main;
            if (cam == null) return;

       
            float distance = Vector3.Distance(missile.position, player.position);
            float t = Mathf.InverseLerp(maxDistance, 0f, distance);
            missileMarker.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, t);

            Vector3 hud = MathHelpers.TransformToHUDSpace(missile.position, cam);
            Vector2 pos = new Vector2(hud.x, hud.y);

            float halfW = Screen.width * horizontalLimit;
            float halfH = Screen.height * verticalLimit;

            if (hud.z > 0f)
            {
              
                pos.x = Mathf.Clamp(pos.x, -halfW, halfW);
                pos.y = Mathf.Clamp(pos.y, -halfH, halfH);
            }
            else
            {
        
                pos = -pos;
                if (pos.sqrMagnitude < 1f) pos = Vector2.down; 

                float kx = halfW / Mathf.Max(Mathf.Abs(pos.x), 0.0001f);
                float ky = halfH / Mathf.Max(Mathf.Abs(pos.y), 0.0001f);
                pos *= Mathf.Min(kx, ky);
            }

            missileMarker.anchoredPosition = pos;
        }
    }
}
