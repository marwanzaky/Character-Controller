using UnityEngine;
using MarwanZaky.Methods;

namespace MarwanZaky
{
    public class LookAtCamera : MonoBehaviour
    {
        Transform cam;

        [SerializeField] Vector3 offset;
        [SerializeField] bool onLateUpdate;
        [SerializeField] bool freezeX, freezeY, freezeZ;

        private void Awake()
        {
            cam = Camera.main.transform;
        }

        private void LateUpdate()
        {
            Refresh();
        }

        private void Refresh()
        {
            transform.rotation = CameraDirection() * Quaternion.Euler(offset);
        }

        Quaternion CameraDirection()
        {
            var dir = transform.position - cam.position;
            var res = Quaternion.LookRotation(dir);

            if (freezeX)
                res = QuaternionX.OverwriteX(res, transform.eulerAngles.x);
            if (freezeY)
                res = QuaternionX.OverwriteY(res, transform.eulerAngles.y);
            if (freezeZ)
                res = QuaternionX.OverwriteZ(res, transform.eulerAngles.z);

            return res;
        }
    }
}