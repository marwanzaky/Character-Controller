using UnityEngine;
using UnityEngine.AI;
using MarwanZaky.Methods;
using System.Linq;

namespace MarwanZaky
{
    public class Enemy : Character
    {
        Vector3 startPos = Vector3.zero;
        Vector3 startRot = Vector3.zero;

        [Header("Enemy"), SerializeField] float visionRadius;
        [SerializeField] string playerTag;
        [SerializeField] LayerMask playerMask;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] float minY;

        protected override void OnEnable()
        {
            healthBar.OnOutOfHealth += Die;
        }

        protected override void OnDisable()
        {
            healthBar.OnOutOfHealth -= Die;
        }

        protected override void Start()
        {
            startPos = transform.position;

            agent.speed = walkSpeed;

            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            FollowTarget();
        }

        private void FollowTarget()
        {
            var position = Vector3X.IgnoreY(transform.position, minY);
            var cols = Physics.OverlapSphere(position, visionRadius, playerMask);
            var taregetCol = cols.FirstOrDefault(el => el.tag == playerTag);

            if (taregetCol != null)
                agent.SetDestination(taregetCol.transform.position);
            else agent.SetDestination(startPos);
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Vector3X.IgnoreY(transform.position, minY), visionRadius);
        }
    }
}