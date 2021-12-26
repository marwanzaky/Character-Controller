using UnityEngine;

namespace MarwanZaky
{
    public static class RaycastHitX
    {
        public static RaycastHit Cast(Vector3 origin, Vector3 direction, LayerMask layerMask, float maxDistance = 10f, bool debug = false)
        {
            var ray = new Ray(origin, direction);
            var hit = new RaycastHit();

            Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, layerMask);

            if (debug)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            }

            return hit;
        }
    }
}