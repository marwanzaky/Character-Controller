using UnityEngine;

namespace Packtool
{
    public class Character : MonoBehaviour
    {
        protected bool IsGrounded(Collider collider, LayerMask whatIsGround, float checkDistance, bool debug = false)
        {
            var originY = transform.position.y + 1f;
            var origin = Vector3X.IgnoreY(transform.position, originY);
            var maxDistance = checkDistance + originY - collider.bounds.min.y;
            var hit = RaycastHitX.Cast(origin, Vector3.down, whatIsGround, maxDistance, debug);
            return hit.collider != null;
        }
    }
}