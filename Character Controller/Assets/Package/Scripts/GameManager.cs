using UnityEngine;

namespace Packtool
{
    public class GameManager : MonoBehaviour
    {
        #region Singletone

        public static GameManager Instance { get; private set; }

        void Singletone()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }

        #endregion

        void Awake()
        {
            Singletone();
            SetTargetFPS();
        }

        void SetTargetFPS()
        {
            const int TARGET_FRAME_RATE = 120;
            Application.targetFrameRate = TARGET_FRAME_RATE;
        }
    }
}