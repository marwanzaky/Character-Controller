using UnityEngine;

namespace MarwanZaky
{
    public class Weapon : MonoBehaviour
    {
        const string ATTACK_ANIM = "Attack";
        [SerializeField] Animator animator;

        protected bool IsAttacking => animator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_ANIM);

        private void OnEnable()
        {
            PlayerMovement.OnAttack += Attack;
        }

        private void OnDisable()
        {
            PlayerMovement.OnAttack -= Attack;
        }

        protected virtual void Attack()
        {
            if (!IsAttacking)
                animator.SetTrigger(ATTACK_ANIM);
        }
    }
}