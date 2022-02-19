using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void TakeDamage(int DamageAmmount);
    public void Die();
}

namespace Player.Stats
{
    [System.Serializable]
    public class PlayerStats
    {
        public int Health = 100;


        [HideInInspector]
        public bool isDead = false;

        public bool CheckHealth()
        {
            return Health > 0;
        }

        public void GameOver()
        {

        }
    }
}
