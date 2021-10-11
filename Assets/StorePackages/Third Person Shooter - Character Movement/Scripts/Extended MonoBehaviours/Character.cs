using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Packtool
{
    public class Character : MonoBehaviour
    {
        public enum GroundCheck { Ray, Sphere, Cube }

        #region Protected Properites

        protected bool IsGrounded { get; private set; } = false;
        protected bool WasGrounded { get; set; } = false;

        #endregion

        #region Private Properties

        Vector3 FeetsCenterPos
        {
            get => Vector3X.IgnoreY(characterCollider.bounds.center, characterCollider.bounds.min.y);
        }

        float GroundRadius
        {
            get => characterCollider.bounds.size.x / 2f;
        }

        #endregion

        [Header("Character Settings")]
        public bool debug = false;
        public float groundDistance = .02f;
        public GroundCheck groundCheck;
        public LayerMask groundMask;
        public Collider characterCollider;

        public virtual void Update()
        {
            IsGrounded = false;

            if (groundCheck == GroundCheck.Ray)
                IsGrounded = IsGroundedCheckRay(debug);
            else if (groundCheck == GroundCheck.Sphere)
                IsGrounded = IsGroundedCheckSphere(debug);
            else if (groundCheck == GroundCheck.Cube)
                IsGrounded = IsGroundedCheckSphere(debug);

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
                characterCollider.bounds.center + Vector3.right * -GroundRadius, // LEFT
                characterCollider.bounds.center + Vector3.right * GroundRadius,   // RIGHT
                characterCollider.bounds.center + Vector3.forward * -GroundRadius,    // BACKWARD
                characterCollider.bounds.center + Vector3.forward * GroundRadius,   // FOREWARD
            };

            var maxDistance = groundDistance + characterCollider.bounds.center.y - characterCollider.bounds.min.y;
            var hits = new List<RaycastHit>();

            foreach (var el in origins)
                hits.Add(RaycastHitX.Cast(el, Vector3.down, groundMask, maxDistance, debug));
            return hits.Select(el => el.collider != null).Contains(true);
        }

        protected bool IsGroundedCheckSphere(bool debug = false)
        {
            if (groundCheck == GroundCheck.Sphere)
                return Physics.CheckSphere(FeetsCenterPos, GroundRadius, groundMask);
            else if (groundCheck == GroundCheck.Cube)
                return Physics.CheckBox(FeetsCenterPos, Vector3.one * GroundRadius, Quaternion.identity, groundMask);

            return false;
        }

        void OnDrawGizmos()
        {
            if (debug)
            {
                if (groundCheck == GroundCheck.Sphere)
                    Gizmos.DrawWireSphere(FeetsCenterPos, GroundRadius);
                else if (groundCheck == GroundCheck.Cube)
                    Gizmos.DrawWireCube(FeetsCenterPos, Vector3.one * GroundRadius * 2f);
            }
        }
    }
}