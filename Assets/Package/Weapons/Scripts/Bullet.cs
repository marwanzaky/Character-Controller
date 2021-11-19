using UnityEngine;

namespace MarwanZaky
{
    public class Bullet : DestroyByTime
    {
        [SerializeField] float speed = 30f;
        [SerializeField] GameObject destroyPrefab;

        private void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            DestroyObject();
        }

        void DestroyObject()
        {
            Instantiate(destroyPrefab, transform.position, Quaternion.identity);
        }
    }
}