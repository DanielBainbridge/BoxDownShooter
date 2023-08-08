using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Enemy
{

    public class EnemyBase : MonoBehaviour
    {
        [Rename("Enemy Health")]public float f_baseHealth = 100;
        private float f_currentHealth = 100;
        [Rename("Enemy Size")]public float f_enemySize = 0.4f;
        [Rename("Collision Bounce Percentage"), Range(0, 1)] public float f_collisionBounciness = 0.45f;

        [Rename("Lock Enemy Position")] public bool b_lockEnemyPosition;

        [Rename("Invincible Time")] public float f_invincibleTime = 0.05f;

        private Vector3 S_velocity;

        private int i_bulletLayerMask;


        //Kyles Enemy Variables
        private GameObject player;
        public GameObject bullet;
        public Transform barrel;
        public float reloadTime;
        public float force;
        [SerializeField]private bool canShoot;

        public enum EnemyState
        {
            Normal,
            Invincible,
            Dodge,
            NoControl,
            NoAttack,
            Slowed,
            Burn,
            Chained,
            Count
        }
        [HideInInspector] public EnemyState e_enemyState;



        private void Start()
        {
            e_enemyState = EnemyState.Normal;
            f_currentHealth = f_baseHealth;
            i_bulletLayerMask = ~(LayerMask.GetMask("Bullet") + LayerMask.GetMask("Ignore Raycast"));

            //Kyles
            player = GameObject.Find("TestCharacter");
            canShoot = true;
        }

        private void Update()
        {
            if (f_currentHealth <= 0)
            {
                gameObject.SetActive(false);

                Invoke("Revive", 10);
                return;
            }
            if (!b_lockEnemyPosition)
            {
                MoveEnemy();
            }


            //Kyles Enemy Code
            gameObject.transform.LookAt(player.transform.position);
            if (canShoot) Shoot();

        }

        private void MoveEnemy()
        {
            transform.localPosition += S_velocity * Time.deltaTime;
            CheckCollisions();
        }

        public void DamageEnemy(float damage)
        {
            f_currentHealth -= damage;
            e_enemyState = EnemyState.Invincible;
            Invoke("NormalizeState", f_invincibleTime);
        }

        private void Revive()
        {
            S_velocity = Vector3.zero;
            gameObject.SetActive(true);
            f_currentHealth = f_baseHealth;
        }

        private void CheckCollisions()
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.localPosition, f_enemySize, Vector3.right, out hit, f_enemySize, i_bulletLayerMask) && S_velocity.x > 0)
            {
                S_velocity.x = -S_velocity.x * f_collisionBounciness;
            }
            else if (Physics.SphereCast(transform.localPosition, f_enemySize, -Vector3.right, out hit, f_enemySize, i_bulletLayerMask) && S_velocity.x < 0)
            {
                S_velocity.x = -S_velocity.x * f_collisionBounciness;
            }
            if (Physics.SphereCast(transform.localPosition, f_enemySize, Vector3.forward, out hit, f_enemySize, i_bulletLayerMask) && S_velocity.z > 0)
            {
                S_velocity.z = -S_velocity.z * f_collisionBounciness;
            }
            else if (Physics.SphereCast(transform.localPosition, f_enemySize, -Vector3.forward, out hit, f_enemySize, i_bulletLayerMask) && S_velocity.z < 0)
            {
                S_velocity.z = -S_velocity.z * f_collisionBounciness;
            }
        }

        public void AddVelocityToEnemy(Vector3 velocityToAdd)
        {
            S_velocity += velocityToAdd;
        }

        private void NormalizeState()
        {
            e_enemyState = EnemyState.Normal;
            Debug.Log("Enemy State is now Normal");
        }



        //Kyle's Basic Enemy Look at player and Shoot
        private void Shoot()
        {
            //if (canShoot)
           // {
                canShoot = false;
                var clone = Instantiate(bullet, barrel.position, Quaternion.identity);
                clone.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
                //Invoke("Reload", reloadTime);
            //}   
        }

        private void Reload()
        {
            canShoot = true;
        }

    }
}