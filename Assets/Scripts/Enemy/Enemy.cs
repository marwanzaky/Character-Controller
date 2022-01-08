using UnityEngine;
using UnityEngine.AI;
using MarwanZaky.Methods;
using System.Linq;

namespace MarwanZaky
{
    public class Enemy : Character
    {
        float nextAttackDelay = 1f;
        float nextAttackTimer = 0;

        Vector3 startPos = Vector3.zero;

        [Header("Enemy"), SerializeField] NavMeshAgent agent;
        [SerializeField] Collider col;
        [SerializeField] float visionRadius;
        [SerializeField] float attackDistance = 2f;
        [SerializeField] string targetTag;
        [SerializeField] LayerMask targetMask;

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

            FollowTarget();

            Animator_MoveY = agent.velocity.magnitude > 0 ? 1f : 0f;
        }

        protected override void Attack()
        {
            if (nextAttackTimer > 0) { return; }

            nextAttackTimer = AttackLength + nextAttackDelay;

            base.Attack();
        }

        private void FollowTarget()
        {
            var position = Vector3X.IgnoreY(transform.position, col.bounds.min.y);
            var targetCols = Physics.OverlapSphere(position, visionRadius, targetMask);
            var targetCol = targetCols.FirstOrDefault(el => el.tag == targetTag && el.GetComponent<Character>().IsAlive);

            if (targetCol != null)
            {
                var dis = Vector3.Distance(transform.position, targetCol.transform.position);

                if (dis > attackDistance)
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