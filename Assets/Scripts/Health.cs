using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class Health : MonoBehaviour, IDamageable
    {
        public delegate void OnPlayerDeath();
        public static event OnPlayerDeath onPlayerDeath;

        public delegate void OnPlayerHealthChanged(int health, int maxHealth);
        public static event OnPlayerHealthChanged onPlayerHealthChanged;

        [SerializeField]
        private int maxHealth;

        private int curHealth;

        private void Start()
        {
            curHealth = maxHealth;
            onPlayerHealthChanged?.Invoke(curHealth, maxHealth);
        }

        public void TakeDamage(int damage)
        {
            curHealth -= damage;
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
            onPlayerHealthChanged?.Invoke(curHealth, maxHealth);
            if (curHealth <= 0)
            {
                onPlayerDeath?.Invoke();
            }
        }

        public void HealDamage(int damage)
        {
            curHealth += damage;
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
            onPlayerHealthChanged?.Invoke(curHealth, maxHealth);
            if (curHealth <= 0)
            {
                onPlayerDeath?.Invoke();
            }
        }
    }
}