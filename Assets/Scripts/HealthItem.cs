using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
