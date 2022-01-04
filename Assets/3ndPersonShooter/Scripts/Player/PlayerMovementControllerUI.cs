using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarwanZaky
{
    public class PlayerMovementControllerUI : MonoBehaviour
    {
        [SerializeField] Image iconImage;
        [SerializeField] TextMeshProUGUI text;

        private void Start()
        {
            text.text = (transform.GetSiblingIndex() + 1).ToString();
        }

        public void Select()
        {
            iconImage.transform.localScale = Vector3.one * 1.1f;
        }

        public void Unselect()
        {
            iconImage.transform.localScale = Vector3.one;
        }
    }
}