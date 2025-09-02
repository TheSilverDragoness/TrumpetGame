using UnityEngine;

namespace DootEmUp.Gameplay
{
    public class HealthItem : MonoBehaviour
    {
        [SerializeField]
        private int healAmount;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageableObject))
            {
                damageableObject.HealDamage(healAmount);
                Destroy(gameObject);
            }
        }
    }
}