using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DootEmUp.Gameplay.Player
{
    public class Health : MonoBehaviour, IDamageable
    {
        public delegate void OnPlayerHealthChanged(int health, int maxHealth);
        public static event OnPlayerHealthChanged onPlayerHealthChanged;

        [SerializeField]
        private UnityEvent onDeath;

        [SerializeField]
        private int maxHealth;

        private int curHealth;

        private void Start()
        {
            if (onDeath == null)
            {
                onDeath = new UnityEvent();
            }

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
                onDeath.Invoke();
            }
        }

        public void HealDamage(int damage)
        {
            curHealth += damage;
            curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
            onPlayerHealthChanged?.Invoke(curHealth, maxHealth);
        }
    }
}