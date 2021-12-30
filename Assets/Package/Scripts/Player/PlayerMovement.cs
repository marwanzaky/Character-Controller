using UnityEngine;
using MarwanZaky.Methods;
using MarwanZaky.Audio;

namespace MarwanZaky
{
    public enum MoveAir { Moveable, NotMoveable }

    public class PlayerMovement : Character
    {
        #region Singletone

        public static PlayerMovement Instance { get; private set; }

        private void Singletone()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }

        #endregion

        InputMaster controlls;

        const float GRAVITY = -9.81f;

        Vector3 velocity = Vector3.zero;

        bool isGrounded = false;
        bool wasGrounded = false;

        Transform cam;

        Vector3 input;
        Vector3 smoothMove;
        Vector3 smoothMoveVelocity;

        Collider col;

        [Header("Player"), SerializeField] protected CharacterController controller;
        [SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None;
        [SerializeField] MoveAir moveAir = MoveAir.Moveable;
        [SerializeField] protected LayerMask groundMask;
        [SerializeField] protected float jumpHeight = 8f;
        [SerializeField] protected float gravityScale = 1f;
        [SerializeField] protected float smoothMoveTime = .2f;

        public float Speed => IsRunning ? runSpeed : walkSpeed;
        public bool IsRunning { get; set; }

        private void Awake()
        {
            Singletone();

            controlls = new InputMaster();

            controlls.Player.Run.started += (res) => IsRunning = true;
            controlls.Player.Run.canceled += (res) => IsRunning = false;

            controlls.Player.Attack.performed += (res) => Attack();
            controlls.Player.Jump.performed += (res) =>
            {
                if (isGrounded)
                    Jump();
            };
        }

        protected override void OnEnable()
        {
            controlls.Enable();

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            controlls.Disable();

            base.OnDisable();
        }

        protected override void Start()
        {
            Cursor.lockState = cursorLockMode;
            cam = Camera.main.transform;

            col = controller.GetComponent<Collider>();

            base.Start();
        }

        protected override void Update()
        {
            input = controlls.Player.Movement.ReadValue<Vector2>();

            base.Update();

            IsGrounded();
            Gravity();

            if (input.magnitude > 0)
                LookAtCamera();

            Movement();
            Inputs();
        }

        private void Inputs()
        {
            // Scroll between controllers
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                UseNextController();
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                UsePreviousController();

            // Toggle cursor lock state
            if (Input.GetKeyDown(KeyCode.M))
                ToggleCursorLockState();

            // Switch controllers.
            if (Input.GetKeyDown(KeyCode.Alpha1))
                OnCurrentControllerChange?.Invoke(0);

            else if (Input.GetKeyDown(KeyCode.Alpha2))
                OnCurrentControllerChange?.Invoke(1);

            else if (Input.GetKeyDown(KeyCode.Alpha3))
                OnCurrentControllerChange?.Invoke(2);
        }

        private void Movement()
        {
            if (!isGrounded && moveAir == MoveAir.NotMoveable) return;

            var move = (transform.right * input.x + transform.forward * input.y).normalized;
            smoothMove = Vector3.SmoothDamp(smoothMove, move, ref smoothMoveVelocity, smoothMoveTime);

            Animator_MoveX = GetAnimMoveVal(input.x);
            Animator_MoveY = GetAnimMoveVal(input.y);

            controller.Move(smoothMove * Speed * Time.deltaTime);
        }

        protected override void Move()
        {
            controller.Move(velocity * Time.deltaTime);
        }

        private void IsGrounded()
        {
            isGrounded = IsGroundedSphere(col, controller.radius, groundMask, true);

            if (isGrounded && velocity.y < 0)
                velocity.y = -5f;

            if (isGrounded && !wasGrounded) // on land
                AudioManager.Instance.Play("Land");

            wasGrounded = isGrounded;
            animator.SetBool("Float", !isGrounded);
        }

        private void Gravity()
        {
            velocity.y = velocity.y + GRAVITY * gravityScale * Time.deltaTime;
        }

        private void Jump()
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY * gravityScale);
            AudioManager.Instance.Play("Jump");
        }

        private void OnGUI()
        {
            var buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 18;

            if (GUI.Button(new Rect(50, 32, 200, 32), $"Cursor Locked (M)", buttonStyle))
                ToggleCursorLockState();

            if (GUI.Button(new Rect(50, 69, 200, 32), "Movement (1)", buttonStyle))
                OnCurrentControllerChange?.Invoke(0);

            if (GUI.Button(new Rect(50, 106, 200, 32), "Sword (2)", buttonStyle))
                OnCurrentControllerChange?.Invoke(1);

            if (GUI.Button(new Rect(50, 143, 200, 32), "Gun (3)", buttonStyle))
                OnCurrentControllerChange?.Invoke(2);
        }

        private void LookAtCamera()
        {
            const float SMOOTH_TIME = 5f;
            var camAngles = Vector3X.IgnoreXZ(cam.eulerAngles);
            var targetRot = Quaternion.Euler(camAngles);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, SMOOTH_TIME * Time.deltaTime);
        }

        private void ToggleCursorLockState() =>
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;

        private float GetAnimMoveVal(float move)
        {
            const float WALK_VAL = 1f;
            const float RUN_VAL = 2f;
            return move * (IsRunning ? RUN_VAL : WALK_VAL);
        }
    }
}
