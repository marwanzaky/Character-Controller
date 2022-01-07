using UnityEngine;

namespace MarwanZaky
{
    public class Ragdoll : MonoBehaviour
    {
        #region Public Properties

        public Animator animator;
        public Collider _collider;
        public Rigidbody _rigidbody;

        #endregion

        #region Private Properties

        Rigidbody[] rbs;
        Collider[] cols;
        CharacterJoint[] characterJoints;

        public Rigidbody[] Rigidbodies => rbs;
        public Collider[] Colliders => cols;
        public CharacterJoint[] CharacterJoints => characterJoints;

        #endregion

        public void Init()
        {
            rbs = GetComponentsInChildren<Rigidbody>();
            cols = GetComponentsInChildren<Collider>();
            characterJoints = GetComponentsInChildren<CharacterJoint>();
        }

        public void Die()
        {
            Active(true);
        }

        public void Revive()
        {
            Active(false);
        }

        #region Private Methods

        void Start()
        {
            Init();
        }

        void Active(bool value)
        {
            animator.enabled = !value;

            foreach (var rb in rbs)
                rb.isKinematic = !value;

            foreach (var c in cols)
                c.enabled = value;

            if (_collider != null)
                _collider.enabled = !value;

            if (_rigidbody != null)
                _rigidbody.isKinematic = value;
        }

        #endregion
    }
}