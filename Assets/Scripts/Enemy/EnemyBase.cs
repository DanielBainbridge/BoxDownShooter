using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{

    public class EnemyBase : MonoBehaviour
    {
        public int baseHealth = 100;
        private int currentHealth = 100;


        private void Start()
        {
            currentHealth = baseHealth;
        }

        private void Update()
        {
            if (currentHealth <= 0)
            {
                gameObject.SetActive(false);

                Invoke("Revive", 1.5f);
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
        }

        private void Revive()
        {
            gameObject.SetActive(true);
            currentHealth = baseHealth;
        }
    }
}