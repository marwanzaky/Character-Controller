using UnityEngine;
using MarwanZaky.Audio;
using System;

namespace MarwanZaky
{
    public class Character : MarwanZaky.Shared.Character
    {
        protected const bool DEBUG = true;

        protected int currentController = 0;
        protected int defaultController = 0;

        [Header("Character"), SerializeField] protected Animator animator;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] protected float walkSpeed = 5f;
        [SerializeField] protected float runSpeed = 10f;
        [SerializeField] protected AnimatorOverrideController[] controllers;

        public bool IsAlive { get; set; }

        public Action OnAttack { get; set; }
        public Action<int> OnCurrentControllerChange { get; set; }

        public float Health
        {
            get
            {
                var res = healthBar.Health;
                return res;
            }

            set
            {
                if (value > 0)
                    healthBar.Health = value;
                else if (IsAlive)
                {
                    IsAlive = false;
                    OnDie();
                }
            }
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
            IsAlive = Health > 0;

            UpdateCurrentController(defaultController);
        }

        protected virtual void Update()
        {
            if (IsAlive)
                Alive();
        }

        protected virtual void Attack()
        {
            animator.SetTrigger("Attack");
            OnAttack?.Invoke();
        }

        protected virtual void Alive()
        {

        }

        protected virtual void OnDie()
        {
            animator.SetTrigger("Die");
            healthBar.gameObject.SetActive(false);
        }

        public virtual void Damage(float damage, Vector3 hitPoint)
        {
            Health -= damage;
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