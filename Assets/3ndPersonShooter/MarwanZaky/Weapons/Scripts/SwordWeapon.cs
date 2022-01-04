using UnityEngine;

namespace MarwanZaky
{
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
            if (character.IsAttack && col.CompareTag(targetTag))
                col.GetComponent<Character>().Damage(damage, transform.position);
        }
    }
}