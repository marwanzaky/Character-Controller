using UnityEngine;
using System.Collections.Generic;

namespace MarwanZaky
{
    public class PlayerMovementControllersUI : MonoBehaviour
    {
        [SerializeField] List<GameObject> controllers;

        private void Start()
        {
            // GetAllControllers();

            PlayerMovement.OnCurrentControllerChange += CurrentController;
        }

        private void OnDisable()
        {
            PlayerMovement.OnCurrentControllerChange -= CurrentController;
        }

        public void CurrentController(int controller)
        {
            DisableAllControllers();
            controllers[controller].SetActive(true);
        }

        void DisableAllControllers()
        {
            for (int i = 0; i < controllers.Count; i++)
                controllers[i].SetActive(false);
        }

        void GetAllControllers()
        {
            foreach (Transform el in transform)
                controllers.Add(el.gameObject);
        }
    }
}