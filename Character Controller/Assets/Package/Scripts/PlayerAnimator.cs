using UnityEngine;

namespace Packtool
{
    public class PlayerAnimator : MonoBehaviour
    {
        public Animator animator;
        public float movementSpeed = 1f;
        public bool transition = false;
        public float transitionSmoothTime = 10f;

        public void UpdateSettings()
        {
            MovementSpeed = movementSpeed;
        }

        public float Movement
        {
            get
            {
                return animator.GetFloat("Movement");
            }
            set
            {
                animator.SetFloat("Movement", transition ? TransitionSmoothTime(value, "Movement") : value);
            }
        }

        public float Direction
        {
            set
            {
                animator.SetFloat("Direction", transition ? TransitionSmoothTime(value, "Direction") : value);
            }
        }

        public float MovementSpeed
        {
            set
            {
                animator.SetFloat("MovementSpeed", transition ? TransitionSmoothTime(value, "MovementSpeed") : value);
            }
        }

        public bool Float
        {
            set
            {
                animator.SetBool("Float", value);
            }
        }

        public bool Idle
        {
            get
            {
                const float MIN_VALUE = .05f;
                return Movement <= MIN_VALUE;
            }
        }

        float TransitionSmoothTime(float value, string name)
        {
            return Mathf.Lerp(animator.GetFloat(name), value, transitionSmoothTime * Time.deltaTime);
        }
    }
}