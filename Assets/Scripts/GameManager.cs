
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

namespace MarwanZaky
{
    public class GameManager : MonoBehaviour
    {
        #region Singletone

        public static GameManager Instance { get; private set; }

        private void Singletone()
        {
            if (Instance == null)
                Instance = this;
            else Destroy(gameObject);
        }

        #endregion

        public Action OnGameOver { get; set; }

        [Header("Unity Events"), SerializeField] UnityEvent onStartUnityEvent;
        [SerializeField] UnityEvent onGameOverUnityEvent;

        private void Awake()
        {
            Singletone();

            Application.targetFrameRate = 60;
        }

        private void OnEnable()
        {
            OnGameOver += GameOver;
        }

        private void OnDisable()
        {
            OnGameOver -= GameOver;
        }

        private void Start()
        {
            onStartUnityEvent?.Invoke();
        }

        private void Update()
        {
#if UNITY_EDITOR
            EDITOR_ONLY();
#endif
        }

        private void GameOver()
        {
            onGameOverUnityEvent?.Invoke();
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void EDITOR_ONLY()
        {
            if (Input.GetKeyDown(KeyCode.R))
                Restart();
        }
    }
}