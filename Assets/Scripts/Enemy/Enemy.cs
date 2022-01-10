using UnityEngine;
using UnityEngine.AI;
using MarwanZaky.Methods;
using System.Linq;

namespace MarwanZaky
{
    public class Enemy : Character
    {
        float targetDis;

        float nextAttackDelay = 1;
        float nextAttackTimer = 0;

        Vector3 startPos = Vector3.zero;

        [Header("Enemy"), SerializeField] NavMeshAgent agent;
        [SerializeField] float visionRadius;
        [SerializeField] float attackDistance = 2f;
        [SerializeField] float walkDistance = 5f;
        [SerializeField] string targetTag;
        [SerializeField] LayerMask targetMask;

        public override bool IsRunning => targetDis > walkDistance ? targetDis != Mathf.Infinity : false;   // if the target distance equals to infinity it means the target is out of his vision

        public override float Radius => agent.radius;
        public override float AttackLength => 1;

        public override Vector3 Move => agent.velocity.magnitude > 0f ? Vector3.up * (IsRunning ? 2f : 1f) : Vector3.zero;

        protected override void Start()
        {
            defaultBehavoir = 1;
            startPos = transform.position;

            base.Start();
        }

        protected override void Alive()
        {
            if (nextAttackTimer > 0)
                nextAttackTimer -= Time.deltaTime;

            IsGrounded();
            Movement();
            FollowTarget();

        }

        protected override void Attack()
        {
            if (nextAttackTimer > 0) { return; }

            nextAttackTimer = AttackLength + nextAttackDelay;

            base.Attack();
        }

        protected override void Movement()
        {
            agent.speed = Speed;

            base.Movement();
        }

        private void FollowTarget()
        {
            var position = Vector3X.IgnoreY(transform.position, col.bounds.min.y);
            var targetCols = Physics.OverlapSphere(position, visionRadius, targetMask);
            var targetCol = targetCols.FirstOrDefault(el => el.tag == targetTag && el.GetComponent<Character>().IsAlive);

            if (targetCol != null)
            {
                targetDis = Vector3.Distance(transform.position, targetCol.transform.position);

                if (targetDis > attackDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(targetCol.transform.position);
                }
                else
                {
                    agent.isStopped = true;
                    Attack();
                }
            }
            else
            {
                targetDis = Mathf.Infinity;
                agent.isStopped = false;
                agent.SetDestination(startPos);
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