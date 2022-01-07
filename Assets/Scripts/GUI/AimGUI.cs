using UnityEngine;

namespace MarwanZaky
{
    public class AimGUI : MonoBehaviour
    {
        [SerializeField] RectTransform top;
        [SerializeField] RectTransform bottom;
        [SerializeField] RectTransform right;
        [SerializeField] RectTransform left;
        [SerializeField] RectTransform point;

        [SerializeField, Range(0f, 100f)] float focus;
        [SerializeField, Range(0f, 1f)] float size;

        private void Update()
        {
            const float VELOCITY_UNIT = .1f;
            var strength = PlayerMovement.Instance.SmoothMove.magnitude * VELOCITY_UNIT + 1f;

            top.anchoredPosition = new Vector2(0f, focus * strength);
            bottom.anchoredPosition = new Vector2(0f, -focus * strength);
            right.anchoredPosition = new Vector2(focus * strength, 0f);
            left.anchoredPosition = new Vector2(-focus * strength, 0f);

            top.sizeDelta = bottom.sizeDelta = new Vector2(15f, 100f) * size;
            right.sizeDelta = left.sizeDelta = new Vector2(100f, 15f) * size;
            point.sizeDelta = new Vector2(20f, 20f) * size;
        }
    }
}