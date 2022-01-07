using UnityEngine;
using MarwanZaky.Audio;
using MarwanZaky.Methods;
using System;
using System.Linq;

namespace MarwanZaky
{
    public class Character : MarwanZaky.Shared.Character
    {
        [System.Serializable]
        public struct Behavoir
        {
            public string name;
            public AnimatorOverrideController controller;
            public GameObject weapon;
            public string attackAudio;
        }

        protected const bool DEBUG = true;

        protected int currentBehavoir = 0;
        protected int defaultBehavoir = 0;

        [Header("Character"), SerializeField] protected Animator animator;
        [SerializeField] private HealthBar healthBar;
        [SerializeField] Ragdoll ragdoll;
        [SerializeField] protected float walkSpeed = 5f;
        [SerializeField] protected float runSpeed = 10f;
        [SerializeField] protected Behavoir[] behavoirs;

        public int DefaultBehavoir => defaultBehavoir;

        public float AttackLength => 2.4f;

        public bool IsAlive { get; set; }
        public bool IsAttack => animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");

        public Action OnAttack { get; set; }
        public Action<int> OnCurrentControllerChange { get; set; }

        public float Health
        {
            get => healthBar.Health;
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
            OnCurrentControllerChange += UpdateCurrentBehavoir;
        }

        protected virtual void OnDisable()
        {
            OnCurrentControllerChange -= UpdateCurrentBehavoir;
        }

        protected virtual void Start()
        {
            IsAlive = Health > 0;

            UpdateCurrentBehavoir(defaultBehavoir);
        }

        protected virtual void Update()
        {
            if (!IsAlive) { return; }

            Alive();
        }

        protected virtual void Attack()
        {
            if (IsAttack) { return; }

            animator.SetTrigger("Attack");
            OnAttack?.Invoke();
        }

        protected virtual void Alive()
        {

        }

        protected virtual void OnDie()
        {
            ragdoll.Die();
            healthBar.gameObject.SetActive(false);
        }

        public virtual void Damage(float damage, Vector3 hitPoint)
        {
            Health -= damage;
            AudioManager.Instance.Play(name: "Hurt", position: transform.position, spatialBlend: 1);
        }

        #region Behavoir

        protected void UseNextBehavoir()
        {
            currentBehavoir = (currentBehavoir + 1) % behavoirs.Length;
            OnCurrentControllerChange?.Invoke(currentBehavoir);
        }

        protected void UsePreviousBehavoir()
        {
            if (currentBehavoir > 0)
                currentBehavoir--;
            else currentBehavoir = behavoirs.Length - 1;

            OnCurrentControllerChange?.Invoke(currentBehavoir);
        }

        protected void UpdateCurrentBehavoir(int currentBehavoir)
        {
            animator.runtimeAnimatorController = behavoirs[currentBehavoir].controller;

            for (int i = 0; i < behavoirs.Length; i++)
                if (behavoirs[i].weapon)
                    behavoirs[i].weapon.SetActive(currentBehavoir == i);
        }

        #endregion

    }
}