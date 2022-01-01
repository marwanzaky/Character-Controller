using UnityEngine;

namespace MarwanZaky
{
    namespace Methods
    {
        public static class DebugX
        {
            public static void DrawVector(Vector3 vector)
            {
                UnityEngine.Debug.DrawRay(vector, vector);
            }
        }
    }
}