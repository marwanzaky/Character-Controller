using UnityEngine;

namespace MarwanZaky
{
    public class GunWeapon : Weapon
    {
        Camera cam;

        [SerializeField] GameObject bulletPrefab;
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
            var aimPos = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
            var ray = Camera.main.ScreenPointToRay(aimPos);
            var hit = new RaycastHit();

            Physics.Raycast(ray.origin, ray.direction, out hit, 1000f, layerMask);

            var targetDir = (hit.point - laser.position).normalized;

            Instantiate(bulletPrefab, laser.position, Quaternion.LookRotation(targetDir));
        }
    }
}