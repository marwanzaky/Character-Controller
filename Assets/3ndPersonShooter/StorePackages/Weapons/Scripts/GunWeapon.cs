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

            if (!character.IsAttack)
                Fire();
        }

        void Fire()
        {
            var mouseHit = RaycastHitX.MouseHit(layerMask);
            var targetDir = Vector3.zero;

            if (mouseHit.hit.collider != null)
                targetDir = (mouseHit.hit.point - laser.position).normalized;
            else targetDir = Camera.main.transform.forward;

            Instantiate(bulletPrefab, laser.position, Quaternion.LookRotation(targetDir));
        }
    }
}