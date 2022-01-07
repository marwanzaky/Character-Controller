using UnityEngine;
using System.Collections.Generic;

namespace MarwanZaky
{
    public class PlayerMovementControllersUI : MonoBehaviour
    {
        [SerializeField] List<PlayerMovementControllerUI> controllers;

        private void Start()
        {
            CurrentController(PlayerMovement.Instance.DefaultBehavoir);
            PlayerMovement.Instance.OnCurrentControllerChange += CurrentController;
        }

        private void OnDisable()
        {
            PlayerMovement.Instance.OnCurrentControllerChange -= CurrentController;
        }

        public void CurrentController(int controller)
        {
            UnselectAllControllers();
            controllers[controller].Select();
        }

        void UnselectAllControllers()
        {
            for (int i = 0; i < controllers.Count; i++)
                controllers[i].Unselect();
        }
    }
}