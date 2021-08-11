using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Packtool
{
    public class Character : MonoBehaviour
    {
        public enum GroundCheck { Sphere, Ray }

        #region Protected Properites

        protected bool IsGrounded { get; set; } = false;
        protected bool WasGrounded { get; set; } = false;

        #endregion

        #region Private Properties

        Vector3 GroundCheckPosition
        {
            get => Vector3X.IgnoreY(characterCollider.bounds.center, characterCollider.bounds.min.y);
        }

        float ColliderRedius
        {
            get => characterCollider.bounds.size.x / 2f;
        }

        #endregion

        [Header("Character Settings")]
        public bool debug = false;
        public float groundDistanceCheck = .02f;
        public GroundCheck groundCheck;
        public LayerMask groundMask;
        public Collider characterCollider;

        public virtual void Update()
        {
            IsGrounded = groundCheck == GroundCheck.Ray ? IsGroundedCheckRay(debug) : IsGroundedCheckSphere(debug);

            if (debug)
            {
                Debug.Log("Is grounded: " + IsGrounded);
            }
        }

        public virtual void FixedUpdate()
        {
            WasGrounded = IsGrounded;
        }

        protected bool IsGroundedCheckRay(bool debug = false)
        {
            var origins = new Vector3[] {
                characterCollider.bounds.center,   // MIDDLE
                characterCollider.bounds.center + Vector3.right * -ColliderRedius, // LEFT
                characterCollider.bounds.center + Vector3.right * ColliderRedius,   // RIGHT
                characterCollider.bounds.center + Vector3.forward * -ColliderRedius,    // BACKWARD
                characterCollider.bounds.center + Vector3.forward * ColliderRedius,   // FOREWARD
        };

            var maxDistance = groundDistanceCheck + characterCollider.bounds.center.y - characterCollider.bounds.min.y;
            var hits = new List<RaycastHit>();

            foreach (var el in origins)
                hits.Add(RaycastHitX.Cast(el, Vector3.down, groundMask, maxDistance, debug));

            return hits.Select(el => el.collider != null).Contains(true);
        }

        protected bool IsGroundedCheckSphere(bool debug = false)
        {
            return Physics.CheckSphere(GroundCheckPosition, ColliderRedius, groundMask);
        }

        void OnDrawGizmos()
        {
            if (debug)
            {
                if (groundCheck == GroundCheck.Sphere)
                {
                    Gizmos.DrawWireSphere(GroundCheckPosition, ColliderRedius);
                }
            }
        }
    }
}