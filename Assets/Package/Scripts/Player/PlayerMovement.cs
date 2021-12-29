using UnityEngine;
using System;

namespace MarwanZaky
{
    public class PlayerMovement : Character
    {
        #region Singletone

        public static PlayerMovement Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else Destroy(gameObject);
        }

        #endregion

        public enum MoveAir { Moveable, NotMoveable }

        Transform cam;

        [Header("Player"), SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None;
        [SerializeField] MoveAir moveAir = MoveAir.Moveable;

        [Space, SerializeField] KeyCode jumpKeyCode = KeyCode.Space;
        [SerializeField] KeyCode runKeyCode = KeyCode.LeftShift;
        [SerializeField] KeyCode attackKeyCode = KeyCode.Mouse0;

        public float Speed => IsRunning ? runSpeed : walkSpeed;
        public bool IsRunning => Input.GetKey(runKeyCode);
        public bool IsMoving { get; set; }

        protected override void Start()
        {
            Cursor.lockState = cursorLockMode;
            cam = Camera.main.transform;

            base.Start();
        }

        private void OnEnable()
        {
            OnCurrentControllerChange += UpdateCurrentController;
        }

        private void OnDisable()
        {
            OnCurrentControllerChange -= UpdateCurrentController;
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

        protected override void Update()
        {
            base.Update();

            if (IsMoving)
                LookAtCamera();

            Movement();
            Inputs();
        }

        private void Inputs()
        {
            if (Input.GetKeyDown(jumpKeyCode) && isGrounded)
                Jump();

            if (Input.GetKeyDown(attackKeyCode))
                Attack();

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

        protected override void Jump()
        {
            base.Jump();
            AudioManager.Instance.Play("Jump");
        }

        private void LookAtCamera()
        {
            const float SMOOTH_TIME = 5f;
            var camAngles = Packtool.Vector3X.IgnoreXZ(cam.eulerAngles);
            var targetRot = Quaternion.Euler(camAngles);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, SMOOTH_TIME * Time.deltaTime);
        }

        private void ToggleCursorLockState() => Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;

        float GetAnimMoveVal(float move, float animCurVal)
        {
            const float SMOOTH_TIME = 10f;
            const float WALK_VAL = 1f;
            const float RUN_VAL = 2f;
            var newVal = move * (IsRunning ? RUN_VAL : WALK_VAL);
            var res = Mathf.Lerp(animCurVal, newVal, SMOOTH_TIME * Time.deltaTime);
            return newVal;
        }
    }
}
