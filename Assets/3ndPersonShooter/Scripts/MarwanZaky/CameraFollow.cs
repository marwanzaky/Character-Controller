using UnityEngine;

namespace MarwanZaky
{
    public class CameraFollow : MonoBehaviour
    {
        float xRotation = 0f;
        float yRotation = 0f;

        public Transform playerBody;
        public Vector3 playerBodyOffset;

        [Header("Other Settings")]
        public float minXRotation = -90f;
        public float maxXRotation = 90f;
        public float mouseSensivity = 100f;

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
            var targetPos = playerBody.position + playerBodyOffset;
            transform.position = targetPos;
        }

        void MoveCamera()
        {
            var mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);

            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}