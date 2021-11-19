using UnityEngine;

namespace MarwanZaky
{
    public class GunWeapon : Weapon
    {
        [SerializeField] GameObject bulletPrefab;
        [SerializeField] Transform laser;

        protected override void Attack()
        {
            base.Attack();

            if (!IsAttacking)
                SpawnBullet();
        }

        void SpawnBullet()
        {
            Instantiate(bulletPrefab, laser.position, laser.rotation);
        }
    }
}