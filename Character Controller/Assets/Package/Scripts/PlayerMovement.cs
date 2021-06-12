using UnityEngine;

namespace Packtool
{
    public class PlayerMovement : Character
    {
        #region Singletone

        public static PlayerMovement Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(this.gameObject);
        }

        #endregion

        bool isGrounded = false;

        public bool Landing { get; set; }

        public PlayerAnimator playerAnimator;
        public Rigidbody _rigidbody;
        public Transform cam;

        [Space] public float speed = 10f;
        public float jumpForce = 350f;
        public float groundMaxDistance = .01f;
        public bool airMove = false;

        [Space] public LayerMask whatIsGround;
        public Collider _collider;

        [Space] public KeyCode keyCodeJump = KeyCode.Space;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            playerAnimator.UpdateSettings();

            IsGrounded();
            Inputs();
            Movement((airMove && !Landing) || (isGrounded && !Landing));

            if (!playerAnimator.Idle)
                LookAtCamera();
        }

        void IsGrounded()
        {
            isGrounded = base.IsGrounded(_collider, whatIsGround, groundMaxDistance, true);
            playerAnimator.Float = !isGrounded;
        }

        void Inputs()
        {
            if (Input.GetKeyDown(keyCodeJump) && isGrounded)
                Jump();
        }

        void Movement(bool move)
        {
            var moveX = move ? Input.GetAxis("Horizontal") : 0f;
            var moveZ = move ? Input.GetAxis("Vertical") : 0f;

            var movement = new Vector3(moveX, 0f, moveZ);
            transform.Translate(movement * Time.deltaTime * speed);

            playerAnimator.Movement = moveZ;
            playerAnimator.Direction = moveX;
        }

        void Jump()
        {
            var addForce = Vector3.up * jumpForce;
            _rigidbody.AddForce(addForce);
        }

        void LookAtCamera()
        {
            const float SMOOTH_TIME = 5f;
            var targetRot = Quaternion.Euler(Vector3X.IgnoreXZ(cam.eulerAngles));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, SMOOTH_TIME * Time.deltaTime);
        }
    }
}