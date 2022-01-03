using UnityEngine;
using MarwanZaky.Methods;
using MarwanZaky.Audio;
using UnityEngine.Animations.Rigging;

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
        [SerializeField] LayerMask groundMask;
        [SerializeField] Transform aimHead;
        [SerializeField] Rig rig;
        [SerializeField] float jumpHeight = 8f;
        [SerializeField] float gravityScale = 1f;
        [SerializeField] float smoothMoveTime = .2f;
        [SerializeField] bool enableGUI = false;

        public float Speed => IsRunning ? runSpeed : walkSpeed;
        public bool IsRunning { get; set; }

        private void Awake()
        {
            Singletone();

            controlls = new InputMaster();

            controlls.Player.Run.started += (res) => IsRunning = true;
            controlls.Player.Run.canceled += (res) => IsRunning = false;

            controlls.Player.Attack.performed += (res) =>
            {
                if (IsAlive)
                    Attack();
            };

            controlls.Player.Jump.performed += (res) =>
            {
                if (isGrounded && IsAlive)
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
            base.Update();
        }

        private void Inputs()
        {
            // Scroll between controllers
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                UseNextBehavoir();
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                UsePreviousBehavoir();

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

        protected override void Alive()
        {
            input = controlls.Player.Movement.ReadValue<Vector2>();

            IsGrounded();
            Gravity();

            if (input.magnitude > 0 && !IsAttack)
                LookAtCamera();

            Inputs();

            if ((isGrounded || moveAir == MoveAir.Moveable) && !IsAttack)
                Movement();

            AimHead();

            controller.Move(velocity * Time.deltaTime);
        }

        protected override void OnDie()
        {
            GameManager.Instance.OnGameOver?.Invoke();
            Cursor.lockState = CursorLockMode.None;

            base.OnDie();
        }

        private void Movement()
        {
            var move = (transform.right * input.x + transform.forward * input.y).normalized;
            smoothMove = Vector3.SmoothDamp(smoothMove, move, ref smoothMoveVelocity, smoothMoveTime);

            Animator_MoveX = GetAnimMoveVal(input.x, Animator_MoveX);
            Animator_MoveY = GetAnimMoveVal(input.y, Animator_MoveY);

            controller.Move(smoothMove * Speed * Time.deltaTime);
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

            if (!enableGUI) { return; }

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

        private void AimHead()
        {
            const float SMOOTH_TIME = .1f;

            var rigWeight = 0;
            var mouseHit = RaycastHitX.MouseHit(groundMask);

            if (mouseHit.hit.collider == null) { return; }

            var playerVector = transform.forward;
            var mouseHitVector = mouseHit.ray.direction.normalized; mouseHitVector.y = 0; mouseHitVector = mouseHitVector.normalized;
            var totalVector = (playerVector + mouseHitVector);

            Debug.DrawRay(transform.position, playerVector);
            Debug.DrawRay(transform.position, mouseHitVector, Color.yellow);
            Debug.DrawRay(transform.position, totalVector, Color.green);

            if (totalVector.magnitude > 1.5f)
            {
                rigWeight = 1;
                aimHead.position = mouseHit.hit.point;
            }

            rig.weight = Mathf.Lerp(rig.weight, rigWeight, SMOOTH_TIME);
        }

        private void ToggleCursorLockState() =>
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;

        private float GetAnimMoveVal(float input, float currentVal)
        {
            const float SMOOTH_TIME = .3f;
            const float WALK_VAL = 1f;
            const float RUN_VAL = 2f;
            var val = input * (IsRunning ? RUN_VAL : WALK_VAL);
            return Mathf.Lerp(currentVal, val, SMOOTH_TIME);
        }
    }
}
