using UnityEngine;
using MarwanZaky.Methods;
using UnityEngine.AI;

namespace MarwanZaky
{
    public class Bullet : DestroyByTime
    {
        Vector3 prevPos;

        [SerializeField] float speed = 30f;
        [SerializeField] GameObject destroyPrefab;
        [SerializeField] LayerMask layerMask;
        [SerializeField] int damage = 10;

        private void Start()
        {
            prevPos = transform.position;
        }

        private void Update()
        {
            var pos = transform.position;
            var dis = Vector3.Distance(pos, prevPos);
            var dir = (pos - prevPos).normalized;
            var hit = RaycastHitX.Cast(prevPos, dir, layerMask, dis, debug: true);

            if (hit.collider != null)
                OnCollide(hit, dir);

            prevPos = pos;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnCollide(RaycastHit hit, Vector3 dir)
        {
            DestroyObject(hit.point, dir);

            if (hit.collider.gameObject.layer == 8)
                hit.collider.GetComponent<Character>().Damage(damage);
        }

        void DestroyObject(Vector3 hitPoint, Vector3 dir)
        {
            Instantiate(destroyPrefab, hitPoint, Quaternion.LookRotation(dir));
            Destroy(gameObject);
        }
    }
}