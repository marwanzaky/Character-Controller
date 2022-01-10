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

        Camera cam;

        Vector3 velocity;
        Vector3 smoothMove;
        Vector3 smoothMoveVel;

        [Header("Player"), SerializeField] protected CharacterController controller;
        [SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None;
        [SerializeField] MoveAir moveAir = MoveAir.Moveable;
        [SerializeField] bool enableGUI = false;
        [SerializeField] float jumpHeight = 8f;
        [SerializeField] float gravityScale = 1f;

        public override Vector3 Move => controlls.Player.Movement.ReadValue<Vector2>();
        public Vector3 SmoothMove => smoothMove;

        public override float Radius => controller.radius;
        public override float AttackLength => 1;

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
            cam = Camera.main;

            base.Start();
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
                this.OnCurrentControllerChange?.Invoke(0);

            else if (Input.GetKeyDown(KeyCode.Alpha2))
                OnCurrentControllerChange?.Invoke(1);

            else if (Input.GetKeyDown(KeyCode.Alpha3))
                this.OnCurrentControllerChange?.Invoke(2);
        }

        protected override void Alive()
        {
            IsGrounded();
            Gravity();

            if (Move.magnitude > 0 && !IsAttack)
                LookAtCamera();

            Inputs();

            if (isGrounded || moveAir == MoveAir.Moveable)
                Movement();

            Rig();

            controller.Move(velocity * Time.deltaTime);
        }

        protected override void Attack()
        {
            if (IsAttack) { return; }

            // LookAtCamera(smoothTime: false);

            base.Attack();
        }

        protected override void OnDie()
        {
            GameManager.Instance.OnGameOver?.Invoke();
            Cursor.lockState = CursorLockMode.None;

            base.OnDie();
        }

        protected override void Movement()
        {
            var move = (transform.right * Move.x + transform.forward * Move.y).normalized;

            smoothMove = Vector3.SmoothDamp(smoothMove, move * Speed, ref smoothMoveVel, smoothMoveTime);

            controller.Move(smoothMove * Time.deltaTime);

            base.Movement();
        }

        protected override void IsGrounded()
        {
            base.IsGrounded();

            if (isGrounded && velocity.y < 0)
                velocity.y = -5f;
        }

        protected override void OnGrounded()
        {
            AudioManager.Instance.Play("Land");
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

        private void LookAtCamera(bool smoothTime = true)
        {
            const float SMOOTH_TIME = 5f;

            var camAngles = Vector3X.IgnoreXZ(cam.transform.eulerAngles);
            var targetRot = Quaternion.Euler(camAngles);

            transform.rotation = smoothTime ? Quaternion.Slerp(transform.rotation, targetRot, SMOOTH_TIME * Time.deltaTime) : targetRot;
        }

        private void Rig()
        {
            const float SMOOTH_TIME = .1f;
            const float MIN_VECTOR_AIM_MEAD = 1f;

            var weight = 0f;
            var mouseHit = RaycastHitX.MouseHit(cam, groundMask, debug: DEBUG);
            var targetPos = mouseHit.ray.direction * 1000f;     // default

            var playerVector = transform.forward;
            var mouseHitVector = mouseHit.ray.direction.normalized; mouseHitVector.y = 0; mouseHitVector = mouseHitVector.normalized;
            var totalVector = (playerVector + mouseHitVector);

            if (DEBUG)
            {
                Debug.DrawRay(transform.position, playerVector);
                Debug.DrawRay(transform.position, mouseHitVector, Color.yellow);
                Debug.DrawRay(transform.position, totalVector, Color.green);
            }

            if (totalVector.magnitude > MIN_VECTOR_AIM_MEAD)
            {
                weight = 1f;
                targetPos = mouseHit.hit.collider ? mouseHit.hit.point : targetPos;
            }
            else weight = 0f;

            rigTarget.position = targetPos;
            rig.weight = Mathf.Lerp(rig.weight, weight, SMOOTH_TIME);
        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == LayerMask.NameToLayer("Collectable"))
                col.GetComponent<ICollect>().Collect();
        }

        #region On GUI

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

        private void ToggleCursorLockState()
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }

        #endregion
    }
}
