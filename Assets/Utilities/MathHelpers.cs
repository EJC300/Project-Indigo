using UnityEngine;
namespace Utilities
{
    public class MathHelpers
    {


        public static Vector3 TransformToHUDSpace(Vector3 pos,Camera camera)
        {
            float halfPixelWidth = camera.pixelWidth / 2;

            float halfPixelHeight = camera.pixelHeight / 2;


           return camera.WorldToScreenPoint(pos) - new Vector3(halfPixelWidth, halfPixelHeight);


        }



    }
}