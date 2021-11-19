using UnityEngine;
using System;

namespace MarwanZaky
{
    public class PlayerMovement : Character
    {
        public enum MoveAir { Moveable, NotMoveable }

        public static Action<int> OnCurrentControllerChange;
        public static Action OnAttack;

        const float GRAVITY = -9.81f;
        const bool DEBUG = true;

        Collider col;
        Transform cam;

        Vector3 velocity = Vector3.zero;

        int currentController = 0;

        bool isGrounded = false;
        bool wasGrounded = false;

        [Header("Properties"), SerializeField] CharacterController controller;
        [SerializeField] Animator animator;
        [SerializeField] float walkSpeed = 5f;
        [SerializeField] float runSpeed = 10f;
        [SerializeField] float gravityScale = 1f;
        [SerializeField] float jumpHeight = 8f;
        [SerializeField] LayerMask groundMask;
        [SerializeField] MoveAir moveAir = MoveAir.Moveable;
        [SerializeField] AnimatorOverrideController[] controllers;

        [Header("Controlls"), SerializeField] KeyCode jumpKeyCode = KeyCode.Space;
        [SerializeField] KeyCode runKeyCode = KeyCode.LeftShift;
        [SerializeField] KeyCode attackKeyCode = KeyCode.Mouse0;

        public float Speed => IsRunning ? runSpeed : walkSpeed;
        public bool IsRunning => Input.GetKey(runKeyCode);
        public bool IsMoving { get; set; }


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            col = controller.GetComponent<Collider>();
            cam = Camera.main.transform;

            UpdateCurrentController();
        }

        private void Update()
        {
            IsGrounded();
            Inputs();

            if (IsMoving)
                LookAtCamera();

            Gravity();
            Movement();

            controller.Move(velocity * Time.deltaTime);
        }

        private void IsGrounded()
        {
            // Is grounded
            isGrounded = IsGroundedSphere(col, controller.radius, groundMask, true);

            if (isGrounded && velocity.y < 0)
                velocity.y = -5f;

            // Play sound fx on land
            if (isGrounded && !wasGrounded)
                AudioManager.Instance.Play("Land");

            wasGrounded = isGrounded;
            animator.SetBool("Float", !isGrounded);
        }

        private void Inputs()
        {
            if (Input.GetKeyDown(jumpKeyCode) && isGrounded)
                Jump();

            if (Input.GetKeyDown(attackKeyCode))
                Attack();

            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                currentController = (currentController + 1) % controllers.Length;
                OnCurrentControllerChange?.Invoke(currentController);
                UpdateCurrentController();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                if (currentController > 0)
                    currentController--;
                else currentController = controllers.Length - 1;

                OnCurrentControllerChange?.Invoke(currentController);
                UpdateCurrentController();
            }
        }

        private void Gravity()
        {
            velocity.y = velocity.y + GRAVITY * gravityScale * Time.deltaTime;
        }

        private void Movement()
        {
            const float IS_MOVING_MIN_MAG = .02f;

            if (moveAir == MoveAir.NotMoveable && !isGrounded)
                return;

            var moveX = Input.GetAxis("Horizontal");
            var moveY = Input.GetAxis("Vertical");
            var move = (transform.right * moveX + transform.forward * moveY).normalized;

            animator.SetFloat("MoveX", GetAnimMoveVal(moveX, animator.GetFloat("MoveX")));
            animator.SetFloat("MoveY", GetAnimMoveVal(moveY, animator.GetFloat("MoveY")));

            controller.Move(move * Speed * Time.deltaTime);
            IsMoving = move.magnitude >= IS_MOVING_MIN_MAG;
        }

        private void Jump()
        {
            AudioManager.Instance.Play("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY * gravityScale);
        }

        void LookAtCamera()
        {
            const float SMOOTH_TIME = 5f;
            var camAngles = Packtool.Vector3X.IgnoreXZ(cam.eulerAngles);
            var targetRot = Quaternion.Euler(camAngles);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, SMOOTH_TIME * Time.deltaTime);
        }

        float GetAnimMoveVal(float move, float animCurVal)
        {
            const float SMOOTH_TIME = 10f;
            const float WALK_VAL = 1f;
            const float RUN_VAL = 2f;
            var newVal = move * (IsRunning ? RUN_VAL : WALK_VAL);
            var res = Mathf.Lerp(animCurVal, newVal, SMOOTH_TIME * Time.deltaTime);
            return newVal;
        }

        void UpdateCurrentController()
        {
            animator.runtimeAnimatorController = controllers[currentController];
        }

        void Attack()
        {
            animator.SetTrigger("Attack");
            OnAttack?.Invoke();
        }
    }
}
