using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private float maxHealth;

        [SerializeField]
        private GameObject healthBar;

        [SerializeField]
        private GameController gameController;

        private float curHealth;

        private void Start()
        {
            curHealth = maxHealth;
        }

        private void Update()
        {
            if (curHealth <= 0)
            {
                Death();
            }
        }

        public void TakeDamage(float amount)
        {
            curHealth -= amount;
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
            UpdateHealthBar();
        }

        public void Heal(float amount)
        {
            curHealth += amount;
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            healthBar.GetComponent<Slider>().value = curHealth / maxHealth;
        }

        private void Death()
        {
            gameController.LoseGame();
        }
    }
}