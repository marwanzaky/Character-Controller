using UnityEngine;

namespace Hobbies {
    public static class QuaternionX {
        public static Quaternion OverwriteX(Quaternion value, float angleX) {
            return Quaternion.Euler(angleX, value.eulerAngles.y, value.eulerAngles.z);
        }

        public static Quaternion OverwriteY(Quaternion value, float angleY) {
            return Quaternion.Euler(value.eulerAngles.x, angleY, value.eulerAngles.z);
        }

        public static Quaternion OverwriteZ(Quaternion value, float angleZ) {
            return Quaternion.Euler(value.eulerAngles.x, value.eulerAngles.y, angleZ);
        }
    }
}