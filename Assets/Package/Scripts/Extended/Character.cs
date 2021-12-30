using UnityEngine;
using MarwanZaky.Audio;
using System;

namespace MarwanZaky
{
    public class Character : MarwanZaky.Shared.Character
    {
        protected const bool DEBUG = true;

        protected int currentController = 0;

        [Header("Character"), SerializeField] protected Animator animator;
        [SerializeField] protected HealthBar healthBar;
        [SerializeField] protected float walkSpeed = 5f;
        [SerializeField] protected float runSpeed = 10f;
        [SerializeField] protected AnimatorOverrideController[] controllers;

        public Action OnAttack { get; set; }
        public Action<int> OnCurrentControllerChange { get; set; }

        public float Health => healthBar.Health;

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
            UpdateCurrentController(0);     // select default controller
        }

        protected virtual void Update()
        {
            Move();
        }

        protected virtual void Attack()
        {
            animator.SetTrigger("Attack");
            OnAttack?.Invoke();
        }

        protected virtual void Move()
        {

        }

        public void Damage(int damage)
        {
            healthBar.Health -= damage;
            AudioManager.Instance.Play(name: "Hurt", position: transform.position, spatialBlend: 1);
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

    }
}