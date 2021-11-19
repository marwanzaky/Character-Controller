using UnityEngine;

namespace MarwanZaky
{
    public class DestroyByTime : MonoBehaviour
    {
        [SerializeField] float lifetime = 1f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}