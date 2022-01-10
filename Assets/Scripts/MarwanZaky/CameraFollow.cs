using UnityEngine;

namespace MarwanZaky
{
    public class CameraFollow : MonoBehaviour
    {
        public enum InputHandling { NewInputSystem, OldInputManager }

        InputMaster controlls;

        float xRotation = 0f;
        float yRotation = 0f;

        public InputHandling inputHandling = InputHandling.NewInputSystem;
        public new Transform camera;
        public Transform playerBody;
        public Vector3 playerBodyOffset;

        [Header("Other Settings")]
        public float minXRotation = -90f;
        public float maxXRotation = 90f;
        public float mouseSensivity = 100f;

        private void Awake()
        {
            controlls = new InputMaster();
        }

        private void OnEnable()
        {
            controlls.Enable();
        }

        private void OnDisable()
        {
            controlls.Disable();
        }

        private void Update()
        {
            MoveCamera();
        }

        private void LateUpdate()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            var targetPos = playerBody.position + playerBodyOffset;
            camera.position = targetPos;
        }

        private void MoveCamera()
        {
            var mouseX = 0f;
            var mouseY = 0f;

            if (inputHandling == InputHandling.NewInputSystem)
            {
                var look = controlls.Player.Look.ReadValue<Vector2>();
                mouseX = look.x * mouseSensivity * Time.deltaTime;
                mouseY = look.y * mouseSensivity * Time.deltaTime;
            }
            else
            {
                mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
                mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
            }


            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);

            camera.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}