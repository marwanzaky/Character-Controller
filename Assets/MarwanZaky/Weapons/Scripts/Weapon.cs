using UnityEngine;
using MarwanZaky.Audio;
using System.Collections;

namespace MarwanZaky
{
    public class Weapon : MonoBehaviour
    {
        const string ATTACK_ANIM = "Attack";

        [Header("Weapon"), SerializeField] protected Character character;
        [SerializeField] protected string audioName;

        private void OnEnable()
        {
            character.OnAttack += Attack;
        }

        private void OnDisable()
        {
            character.OnAttack -= Attack;
        }

        protected virtual void Update()
        {
            if (character.IsAttack)
                OnAttackUpdate();
        }

        private void Attack()
        {
            StopAllCoroutines();
            StartCoroutine(AttackIE());
        }

        protected virtual IEnumerator AttackIE()
        {
            if (character.IsAttack) yield break;

            if (!string.IsNullOrEmpty(audioName))
                AudioManager.Instance.Play(audioName);
        }

        protected virtual void OnAttackUpdate()
        {

        }
    }
}