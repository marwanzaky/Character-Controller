using UnityEngine;
using UnityEngine.AI;
using MarwanZaky.Methods;
using System.Linq;

namespace MarwanZaky
{
    public class Enemy : Character
    {
        const float IN_COMA_DELAY = .3f;
        const float NEXT_ATTACK_DELAY = 1f;

        float inComaTimer = 0;
        float nextAttackTimer = 0;

        Vector3 startPos = Vector3.zero;

        [Header("Enemy"), SerializeField] NavMeshAgent agent;
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] Collider col;
        [SerializeField] string playerTag;
        [SerializeField] float visionRadius;
        [SerializeField] float attackDistance = 2f;
        [SerializeField] bool addForceOnDamage = true;
        [SerializeField] LayerMask playerMask;

        protected override void Start()
        {
            defaultBehavoir = 1;
            startPos = transform.position;
            agent.speed = walkSpeed;

            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void Alive()
        {
            if (nextAttackTimer > 0)
                nextAttackTimer -= Time.deltaTime;

            if (inComaTimer > 0)
            {
                inComaTimer -= Time.deltaTime;
                return;
            }

            InComa();

            FollowTarget();

            Animator_MoveY = agent.velocity.magnitude > 0 ? 1f : 0f;
        }

        public override void Damage(float damage, Vector3 hitPoint)
        {
            base.Damage(damage, hitPoint);

            if (addForceOnDamage)
            {
                const float HIT_FORCE = 500f;
                var hitDir = Vector3X.IgnoreY((transform.position - hitPoint).normalized);

                inComaTimer = IN_COMA_DELAY;
                InComa();   // update

                _rigidbody.AddForce(hitDir * HIT_FORCE);
            }
        }

        protected override void Attack()
        {
            agent.velocity = Vector3.zero;

            if (nextAttackTimer > 0) { return; }

            nextAttackTimer = AttackLength + NEXT_ATTACK_DELAY;
            base.Attack();
        }

        private void FollowTarget()
        {
            var position = Vector3X.IgnoreY(transform.position, col.bounds.min.y);
            var cols = Physics.OverlapSphere(position, visionRadius, playerMask);
            var targetCol = cols.FirstOrDefault(el => el.tag == playerTag && el.GetComponent<Character>().IsAlive);

            if (targetCol != null)
            {
                if (Vector3.Distance(transform.position, targetCol.transform.position) > attackDistance && !IsAttack)
                    agent.SetDestination(targetCol.transform.position);
                else Attack();
            }
            else agent.SetDestination(startPos);
        }

        private void InComa()
        {
            if (inComaTimer > 0)
            {
                agent.enabled = false;
                _rigidbody.useGravity = true;
            }
            else
            {
                agent.enabled = true;
                _rigidbody.useGravity = false;
                _rigidbody.velocity = Vector3.zero;
            }
        }

        protected override void OnDie()
        {
            base.OnDie();

            agent.velocity = Vector3.zero;
            agent.enabled = false;
            Destroy(gameObject, 10f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Vector3X.IgnoreY(transform.position, col.bounds.min.y), visionRadius);
        }
    }
}