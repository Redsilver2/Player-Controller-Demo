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
        public int MaxHealth { get; set; }

        public int Endurance = 100;
        public int MaxEndurance { get; set; }

        public int MoneyAmmount = 0;

        public void Initilize()
        {
            MaxHealth = Health;
            MaxEndurance = Endurance;
        }

        [HideInInspector]
        public bool isDead { get => false; set => isDead = value; }

        public void CheckAll()
        {
            if(Health >= MaxHealth)
            {
                Health = MaxHealth;
            }

            if(Endurance >= MaxEndurance)
            {
                Endurance = MaxEndurance;
            }
        }

        public bool CheckHealth()
        {
                return Health > 0;     
        }

        public void GameOver()
        {

        }
    }
}
