using UnityEngine;
using MarwanZaky.Methods;

namespace MarwanZaky
{
    public class GunWeapon : Weapon
    {
        Camera cam;

        [Header("Gun"), SerializeField] GameObject bulletPrefab;
        [SerializeField] Transform laser;
        [SerializeField] LayerMask layerMask;

        private void Start()
        {
            cam = Camera.main;
        }

        protected override void Attack()
        {
            base.Attack();

            if (!IsAttacking)
                Fire();
        }

        void Fire()
        {
            var hit = RaycastHitX.MouseHit(layerMask);
            var targetDir = Vector3.zero;

            if (hit.collider != null)
                targetDir = (hit.point - laser.position).normalized;
            else targetDir = Camera.main.transform.forward;

            Instantiate(bulletPrefab, laser.position, Quaternion.LookRotation(targetDir));
        }
    }
}