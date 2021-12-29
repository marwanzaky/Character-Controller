using UnityEngine;

namespace MarwanZaky
{
    public class Enemy : Character
    {
        [Header("Enemy"), SerializeField] float radius;
        [SerializeField] LayerMask playerMask;

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
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            RaycastHit hit;
            Physics.SphereCast(transform.position, radius, Vector3.forward, out hit, Mathf.Infinity, playerMask);
            if (hit.collider != null)
                LookAt(hit.collider.transform);
        }

        private void LookAt(Transform target)
        {

        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}