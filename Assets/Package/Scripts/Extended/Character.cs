using UnityEngine;
using System.Collections.Generic;
using Packtool;
using System;

namespace MarwanZaky
{
    public class Character : MonoBehaviour
    {
        protected const float GRAVITY = -9.81f;
        protected const bool DEBUG = true;

        protected Collider col;

        protected int currentController = 0;

        protected Vector3 velocity = Vector3.zero;

        protected bool isGrounded = false;
        protected bool wasGrounded = false;

        [Header("Character"), SerializeField] protected CharacterController controller;
        [SerializeField] protected Animator animator;
        [SerializeField] protected HealthBar healthBar;
        [SerializeField] protected float smoothMoveTime = .2f;
        [SerializeField] protected float walkSpeed = 5f;
        [SerializeField] protected float runSpeed = 10f;
        [SerializeField] protected float gravityScale = 1f;
        [SerializeField] protected float jumpHeight = 8f;

        [Space, SerializeField] protected LayerMask groundMask;
        [SerializeField] protected AnimatorOverrideController[] controllers;

        public Action OnAttack { get; set; }
        public Action<int> OnCurrentControllerChange { get; set; }

        public float Health
        {
            get => healthBar.Health;
            set => healthBar.Health = value;
        }

        protected float Animator_MoveX
        {
            get => animator.GetFloat("MoveX");
            set => animator.SetFloat("MoveX", value);
        }

        protected float Animator_MoveY
        {
            get => animator.GetFloat("MoveY");
            set => animator.SetFloat("MoveY", value);
        }

        protected virtual void OnEnable()
        {
            OnCurrentControllerChange += UpdateCurrentController;
        }

        protected virtual void OnDisable()
        {
            OnCurrentControllerChange -= UpdateCurrentController;
        }

        protected virtual void Start()
        {
            col = controller.GetComponent<Collider>();
            UpdateCurrentController(0);     // select default controller
        }

        protected virtual void Update()
        {
            IsGrounded();
            Gravity();

            controller.Move(velocity * Time.deltaTime);
        }

        protected virtual void Jump()
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY * gravityScale);
        }

        protected virtual void IsGrounded()
        {
            // Is grounded
            isGrounded = IsGroundedSphere(col, controller.radius, groundMask, true);

            if (isGrounded && velocity.y < 0)
                velocity.y = -5f;

            // Play sound fx on land
            if (isGrounded && !wasGrounded)
                OnGrounded();

            wasGrounded = isGrounded;
            animator.SetBool("Float", !isGrounded);
        }

        protected virtual void Attack()
        {
            animator.SetTrigger("Attack");
            OnAttack?.Invoke();
        }

        protected virtual void OnGrounded()
        {

        }

        private void Gravity()
        {
            velocity.y = velocity.y + GRAVITY * gravityScale * Time.deltaTime;
        }

        #region Controller

        protected void UseNextController()
        {
            currentController = (currentController + 1) % controllers.Length;
            OnCurrentControllerChange?.Invoke(currentController);
        }

        protected void UsePreviousController()
        {
            if (currentController > 0)
                currentController--;
            else currentController = controllers.Length - 1;

            OnCurrentControllerChange?.Invoke(currentController);
        }

        protected void UpdateCurrentController(int currentController) =>
            animator.runtimeAnimatorController = controllers[currentController];

        #endregion

        protected bool IsGroundedRaycast(Collider collider, float radius, float groundDistance, LayerMask groundMask, bool debug = false)
        {
            var origins = new Vector3[] {
                collider.bounds.center,   // middle
                collider.bounds.center + Vector3.right * -radius, // left
                collider.bounds.center + Vector3.right * radius,   // right
                collider.bounds.center + Vector3.forward * -radius,    // backward
                collider.bounds.center + Vector3.forward * radius,   // foreward
            };

            var maxDistance = groundDistance + collider.bounds.center.y - collider.bounds.min.y;
            var hits = new List<RaycastHit>();

            foreach (var el in origins)
            {
                var hit = RaycastHitX.Cast(el, Vector3.down, groundMask, maxDistance, debug);
                hits.Add(hit);
            }

            foreach (var el in hits)
            {
                if (el.collider != null)
                    return true;
                continue;
            }

            return false;
        }

        protected bool IsGroundedSphere(Collider collider, float radius, LayerMask groundMask, bool debug = false)
        {
            var groundCheckPos = Vector3X.IgnoreY(transform.position, collider.bounds.min.y);
            return Physics.CheckSphere(groundCheckPos, radius, groundMask);
        }
    }
}