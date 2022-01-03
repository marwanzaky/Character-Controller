using UnityEngine;
using MarwanZaky.Audio;

namespace MarwanZaky
{
    public class Weapon : MonoBehaviour
    {
        const string ATTACK_ANIM = "Attack";

        [Header("Weapon"), SerializeField] Character character;
        [SerializeField] Animator animator;
        [SerializeField] string audioName;

        protected bool IsAttacking => animator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_ANIM);

        private void OnEnable()
        {
            character.OnAttack += Attack;
        }

        private void OnDisable()
        {
            character.OnAttack -= Attack;
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