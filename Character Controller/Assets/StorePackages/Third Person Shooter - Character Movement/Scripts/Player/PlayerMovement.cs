using UnityEngine;

namespace Packtool
{
    public enum Move { Moveable, NotMoveable }

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

        #region Public Properties

        public bool Moveable { get; set; } = true;

        public bool IsMoving
        {
            get => Mathf.Abs(controller.velocity.x) + Mathf.Abs(controller.velocity.z) > Mathf.Epsilon;
        }

        public float Speed
        {
            get => Input.GetKey(runKeyCode) ? runSpeed : walkSpeed;
        }

        #endregion

        Vector3 velocity = Vector3.zero;
        float movementSoundFXLength = 0f;

        [Header("References")]
        public CharacterController controller;
        public Animator animator;
        public Transform _camera;

        [Header("Movement Settings")]
        public float walkSpeed = 5f;
        public float runSpeed = 10f;
        public float gravity = -9.81f;
        public float jumpHeight = 8f;
        public Move air = Move.Moveable;
        public Move land = Move.NotMoveable;

        public KeyCode jumpKeyCode = KeyCode.Space;
        public KeyCode runKeyCode = KeyCode.LeftShift;

        [Header("Sound FXs")]
        public AudioClipsData walkClipsData;
        public AudioClipsData runClipsData;
        public AudioClipsData landClipsData;
        public AudioClipsData jumpClipsData;

        [Header("Animator Settings")]
        [Range(.1f, 5f)] public float animatorMovementSpeed = 1f;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            if (animator)
                animator.SetFloat("MovementSpeed", animatorMovementSpeed);
        }

        public override void Update()
        {
            base.Update();

            Gravity();

            if (air == Move.Moveable || IsGrounded)
                Movement();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (IsMoving)
                LookAtCamera();
        }

        void Gravity()
        {
            const float GROUNDED_VELOCITY = -9.81f;

            if (animator)
                animator.SetBool("Float", !IsGrounded);

            if (IsGrounded && velocity.y < 0)
                velocity.y = GROUNDED_VELOCITY;

            if (!IsGrounded && WasGrounded && velocity.y < 0)
                velocity.y = 0f;

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                if (jumpClipsData.clips.Length > 0)
                    SoundFX(jumpClipsData);

                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            if (IsGrounded && !WasGrounded)
            {
                if (landClipsData.clips.Length > 0)
                    SoundFX(landClipsData);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }

        void Movement()
        {
            if (Moveable)
            {
                var x = Input.GetAxis("Horizontal");
                var y = Input.GetAxis("Vertical");

                var movement = transform.right * x + transform.forward * y;
                controller.Move(movement * Speed * Time.deltaTime);

                if (Mathf.Abs(x) + Mathf.Abs(y) != 0f && IsGrounded)
                {
                    if (movementSoundFXLength <= 0f && walkClipsData.clips.Length > 0 && runClipsData.clips.Length > 0)
                    {
                        var (clip, length) = SoundFX(Speed == walkSpeed ? walkClipsData : runClipsData);
                        movementSoundFXLength = length;
                    }
                    else
                    {
                        movementSoundFXLength -= Time.deltaTime;
                    }
                }

                if (animator)
                {
                    var strength = (Speed == walkSpeed ? 1f : 2f);
                    animator.SetFloat("Movement", y * strength);
                    animator.SetFloat("Direction", x * strength);
                }
            }
        }

        void LookAtCamera()
        {
            const float SMOOTH_TIME = 5f;
            var camAngles = Vector3X.IgnoreXZ(_camera.eulerAngles);
            var targetRot = Quaternion.Euler(camAngles);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, SMOOTH_TIME * Time.deltaTime);
        }

        (AudioClip audioClip, float length) SoundFX(AudioClipsData data)
        {
            var go = new GameObject("Sound fx");
            var audioSource = go.AddComponent<AudioSource>();
            var (clip, length) = data.RandomClip();

            go.transform.position = transform.position;

            audioSource.clip = clip;
            audioSource.volume = data.volume;
            audioSource.pitch = data.pitch;
            audioSource.Play();

            Destroy(go, length);

            return (clip, length);
        }
    }
}