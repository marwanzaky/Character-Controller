using UnityEngine;
using Packtool;

namespace MarwanZaky
{
    public class Character : MonoBehaviour
    {
        protected bool IsGrounded(Collider collider, LayerMask groundMask, bool debug = false)
        {
            const float GROUND_DIS = .1f;
            var maxDistance = GROUND_DIS + collider.bounds.center.y - collider.bounds.min.y;
            var hitInfo = RaycastHitX.Cast(collider.bounds.center, Vector3.down, groundMask, maxDistance, debug);
            return hitInfo.collider != null;
        }

        protected bool IsGroundedSphere(float radius, Collider collider, LayerMask groundMask, bool debug = false)
        {
            var groundCheckPos = Vector3X.IgnoreY(transform.position, collider.bounds.min.y);
            return Physics.CheckSphere(groundCheckPos, radius, groundMask);
        }

        protected void LookAtMovementDirection(ref Vector3 lastPos)
        {
            LookAtMovementDirection(ref lastPos, transform);
        }

        protected void LookAtMovementDirection(ref Vector3 lastPos, Transform target, float offsetAngleY = 0f, float smoothTime = 10f)
        {
            var dir = Vector3X.IgnoreY(transform.position - lastPos);

            if (dir.magnitude > Mathf.Epsilon)
            {
                target.rotation = Quaternion.Slerp(target.rotation, Quaternion.LookRotation(dir), smoothTime * Time.deltaTime) * Quaternion.AngleAxis(offsetAngleY, Vector3.up);
            }

            lastPos = transform.position;
        }
    }
}