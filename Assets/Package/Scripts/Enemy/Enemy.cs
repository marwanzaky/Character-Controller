using UnityEngine;
using UnityEngine.AI;
using MarwanZaky.Methods;
using System.Linq;

namespace MarwanZaky
{
    public class Enemy : Character
    {
        const float IN_COMA_DELAY = .3f;
        float inComa = 0;

        Vector3 startPos = Vector3.zero;

        [Header("Enemy"), SerializeField] NavMeshAgent agent;
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] Collider col;
        [SerializeField] LayerMask playerMask;
        [SerializeField] bool addForceOnDamage = true;
        [SerializeField] string playerTag;
        [SerializeField] float visionRadius;

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
            if (inComa > 0)
            {
                inComa -= Time.deltaTime;
                return;
            }

            base.Update();

            InComa();
            FollowTarget();
        }

        public override void Damage(int damage, Vector3 hitPoint)
        {
            base.Damage(damage, hitPoint);

            if (addForceOnDamage)
            {
                const float HIT_FORCE = 500f;
                var hitDir = Vector3X.IgnoreY((transform.position - hitPoint).normalized);

                inComa = IN_COMA_DELAY;
                InComa();   // update

                _rigidbody.AddForce(hitDir * HIT_FORCE);
            }
        }

        private void FollowTarget()
        {
            var position = Vector3X.IgnoreY(transform.position, col.bounds.min.y);
            var cols = Physics.OverlapSphere(position, visionRadius, playerMask);
            var taregetCol = cols.FirstOrDefault(el => el.tag == playerTag);

            if (taregetCol != null)
                agent.SetDestination(taregetCol.transform.position);
            else agent.SetDestination(startPos);
        }

        private void InComa()
        {
            if (inComa > 0)
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

        private void Die()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Vector3X.IgnoreY(transform.position, col.bounds.min.y), visionRadius);
        }
    }
}