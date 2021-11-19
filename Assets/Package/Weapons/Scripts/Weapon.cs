using UnityEngine;

namespace MarwanZaky
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] Animator animator;

        private void Start()
        {
            PlayerMovement.OnAttack += Attack;
        }

        private void OnDisable()
        {
            PlayerMovement.OnAttack -= Attack;
        }

        protected virtual void Attack()
        {
            animator.SetTrigger("Attack");
        }
    }
}