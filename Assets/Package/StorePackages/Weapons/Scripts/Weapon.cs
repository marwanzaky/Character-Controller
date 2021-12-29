using UnityEngine;

namespace MarwanZaky
{
    public class Weapon : MonoBehaviour
    {
        const string ATTACK_ANIM = "Attack";
        [Header("Weapon"), SerializeField] Animator animator;
        [SerializeField] string audioName;

        protected bool IsAttacking => animator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_ANIM);

        private void OnEnable()
        {
            PlayerMovement.Instance.OnAttack += Attack;
        }

        private void OnDisable()
        {
            PlayerMovement.Instance.OnAttack -= Attack;
        }

        protected virtual void Attack()
        {
            if (IsAttacking) return;

            animator.SetTrigger(ATTACK_ANIM);

            if (!string.IsNullOrEmpty(audioName))
                AudioManager.Instance.Play(audioName);
        }
    }
}