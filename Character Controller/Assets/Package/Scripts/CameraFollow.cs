using UnityEngine;

namespace Packtool
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offsetPosition;

        [Header("Other Settings")]
        public float smoothTime = 10f;
        public float mouseXSpeed = 1f;
        public float mouseYSpeed = 1f;

        void Update()
        {
            MoveCamera();
        }

        void LateUpdate()
        {
            FollowTarget();
        }

        void FollowTarget()
        {
            var targetPos = target.position + offsetPosition;
            transform.position = targetPos;
        }

        void MoveCamera()
        {
            var rotation = Quaternion.Euler(transform.eulerAngles.x + -Input.GetAxis("Mouse Y") * mouseYSpeed,
                                            transform.eulerAngles.y + Input.GetAxis("Mouse X") * mouseXSpeed,
                                            transform.eulerAngles.z);

            transform.rotation = rotation;
        }
    }
}