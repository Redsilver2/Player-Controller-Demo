using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    [SerializeField] int MinDamage = 10;
    [SerializeField] int MaxDamage = 50;

    int DamageAmmount = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision)
        {
            return;
        }

        if(collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            DamageAmmount = Random.Range(MinDamage, MaxDamage + 1);
            damagable.TakeDamage(DamageAmmount);
        }
    }
}
