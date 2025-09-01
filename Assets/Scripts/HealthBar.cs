using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        Health.onPlayerHealthChanged += UpdateHealthSlider;
    }

    private void OnDisable()
    {
        Health.onPlayerHealthChanged -= UpdateHealthSlider;
    }

    private void UpdateHealthSlider(int curHealth, int maxHealth)
    {
        slider.value = (float)curHealth / (float)maxHealth;
        Debug.Log("health bar updated " + curHealth + " / " + maxHealth);
    }

}
