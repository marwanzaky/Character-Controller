using UnityEngine;

namespace MarwanZaky
{
    public class Bullet : DestroyByTime
    {
        Vector3 prevPos;

        [SerializeField] float speed = 30f;
        [SerializeField] GameObject destroyPrefab;
        [SerializeField] LayerMask layerMask;

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
                DestroyObject(hit.point, dir);

            prevPos = pos;
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        void DestroyObject(Vector3 hitPoint, Vector3 dir)
        {
            var go = Instantiate(destroyPrefab, hitPoint, Quaternion.LookRotation(dir));
            // go.transform.rotation = Quaternion.Euler(90, 100, 110);
            Destroy(gameObject);
        }
    }
}