using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBar : MonoBehaviour
{
    const float SMOOTH_TIME = 3f;

    public Action OnOutOfHealth { get; set; }

    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0f, maxHealth);

            if (Health <= 0)
                OnOutOfHealth?.Invoke();
        }
    }

    [SerializeField] float maxHealth = 100f;
    [SerializeField] float health = 100f;
    [SerializeField] bool resetHealth = false;
    [SerializeField] Slider slider;

    [Header("Health Color Settings")]
    [SerializeField] Image fillImage;
    [SerializeField] Color healthyColor = Color.green;
    [SerializeField] Color unHealthyColor = Color.red;

    void Start()
    {
        if (resetHealth)
            Health = maxHealth;
    }

    void Update()
    {
        UpdateSliderValue();
    }

    void UpdateSliderValue()
    {
        var smoothSliderVal = Mathf.Lerp(slider.value, Health, Time.deltaTime * SMOOTH_TIME);
        var smoothSliderCol = Color.Lerp(unHealthyColor, healthyColor, smoothSliderVal / 100f);
        slider.value = smoothSliderVal;
        fillImage.color = smoothSliderCol;
    }
}