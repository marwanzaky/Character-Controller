using UnityEngine;

namespace Packtool
{
    public static class Vector3X
    {
        public static Vector3 IgnoreY(Vector3 value, float y = 0)
        {
            return new Vector3(value.x, y, value.z);
        }

        public static Vector3 IgnoreX(Vector3 value, float x = 0)
        {
            return new Vector3(x, value.y, value.z);
        }

        public static Vector3 IgnoreXZ(Vector3 value)
        {
            return new Vector3(0f, value.y, 0f);
        }
    }
}