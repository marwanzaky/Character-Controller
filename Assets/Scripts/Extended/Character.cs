using UnityEngine;
using MarwanZaky.Audio;
using UnityEngine.Animations.Rigging;
using System;

namespace MarwanZaky
{
    public class Character : MarwanZaky.Shared.Character
    {
        [System.Serializable]
        public struct Behavoir
        {
            [Header("Properties")]
            public string name;
            public AnimatorOverrideController controller;
            public GameObject weapon;
            public string attackAudio;

            [Header("Rig Weight")]
            [Range(0f, 1f)] public float rigChestAimWeight;
            [Range(0f, 1f)] public float rigHandRightAimWeight;
            [Range(0f, 1f)] public float rigHandLeftAimWeight;
            [Range(0f, 1f)] public float rigHeadAimWeight;
        }

        protected const bool DEBUG = true;

        protected int currentBehavoir = 0;
        protected int defaultBehavoir = 0;

        protected bool isGrounded = false;
        protected bool wasGrounded = false;

        protected float smoothAnimValVelX;    // smooth animation value velocity x-axis
        protected float smoothAnimValVelY;    // smooth animation value velcoity y-axis

        [Header("Character Properties"), SerializeField] protected Animator animator;
        [SerializeField] protected Collider col;
        [SerializeField] protected HealthBar healthBar;
        [SerializeField] protected Ragdoll ragdoll;
        [SerializeField] protected LayerMask groundMask;
        [SerializeField] protected float walkSpeed = 3f;
        [SerializeField] protected float runSpeed = 10f;
        [SerializeField] protected float smoothMoveTime = .2f;
        [SerializeField] protected Behavoir[] behavoirs;

        [Header("Character Rig"), SerializeField] protected Rig rig;
        [SerializeField] protected MultiAimConstraint rigChestAim;
        [SerializeField] protected MultiAimConstraint rigHandRightAim;
        [SerializeField] protected TwoBoneIKConstraint rigHandLeftAim;
        [SerializeField] protected MultiAimConstraint rigHeadAim;
        [SerializeField] protected Transform rigTarget;

        public Action OnAttack { get; set; }
        public Action<int> OnCurrentControllerChange { get; set; }

        public virtual Vector3 Move { get; set; }

        public int DefaultBehavoir => defaultBehavoir;

        public virtual bool IsRunning { get; set; }

        public bool IsAlive { get; set; }
        public bool IsAttack => animator.GetCurrentAnimatorStateInfo(layerIndex: 2).IsName("Attack");

        public float Speed => IsRunning ? runSpeed : walkSpeed;

        public virtual float Radius { get; set; }
        public virtual float AttackLength { get; set; }

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

        protected virtual void Movement()
        {
            Animator_MoveX = GetAnimMoveVal(Move.x, Animator_MoveX, ref smoothAnimValVelX);
            Animator_MoveY = GetAnimMoveVal(Move.y, Animator_MoveY, ref smoothAnimValVelY);
        }

        protected virtual void IsGrounded()
        {
            isGrounded = IsGroundedSphere(col, Radius, groundMask, DEBUG);

            if (isGrounded && !wasGrounded)
                OnGrounded();

            wasGrounded = isGrounded;
            animator.SetBool("Float", !isGrounded);
        }

        protected virtual void OnGrounded()
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
            AudioManager.Instance.Play(name: "Hurt", transform.position);
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

            rigChestAim.weight = behavoirs[currentBehavoir].rigChestAimWeight;
            rigHandRightAim.weight = behavoirs[currentBehavoir].rigHandRightAimWeight;
            rigHandLeftAim.weight = behavoirs[currentBehavoir].rigHandLeftAimWeight;
            rigHeadAim.weight = behavoirs[currentBehavoir].rigHeadAimWeight;

            for (int i = 0; i < behavoirs.Length; i++)
                if (behavoirs[i].weapon)
                    behavoirs[i].weapon.SetActive(currentBehavoir == i);
        }

        #endregion

        float GetAnimMoveVal(float move, float currentVal, ref float smoothVal)
        {
            const float WALK_VAL = 1f;
            const float RUN_VAL = 2f;

            var targetVal = move * (IsRunning ? RUN_VAL : WALK_VAL);
            return Mathf.SmoothDamp(currentVal, targetVal, ref smoothVal, smoothMoveTime);
        }
    }
}