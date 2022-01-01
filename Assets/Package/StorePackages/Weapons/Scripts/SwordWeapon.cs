using UnityEngine;
using System.Linq;

namespace MarwanZaky
{
    [RequireComponent(typeof(Rigidbody))]
    public class SwordWeapon : Weapon
    {
        [Header("Sword"), SerializeField] float damage = 10;
        [SerializeField] string targetTag;

        protected override void Attack()
        {
            base.Attack();
        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag(targetTag))
                col.GetComponent<Character>().Damage(damage, transform.position);
        }
    }
}