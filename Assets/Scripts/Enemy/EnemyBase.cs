using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Gun;
using static Gun.GunModule;

namespace Enemy
{

    public class EnemyBase : MonoBehaviour
    {
        [Rename("Enemy Health")] public float f_baseHealth = 100;
        private float f_currentHealth = 100;
        [Rename("Enemy Size")] public float f_enemySize = 0.4f;
        [Rename("Collision Bounce Percentage"), Range(0, 1)] public float f_collisionBounciness = 0.45f;

        [Rename("Lock Enemy Position")] public bool b_lockEnemyPosition;

        [Rename("Invincible Time")] public float f_invincibleTime = 0.05f;

        private Vector3 S_velocity;

        private int i_bulletLayerMask;
        [Range(0, 1)] private float f_slowMultiplier = 1;

        //Kyles Enemy Variables
        private GameObject player;
        public GameObject bullet;
        public Transform barrel;
        public float reloadTime;
        public float force;
        [SerializeField] private bool canShoot;

        public enum EnemyState
        {
            Normal,
            Invincible,
            Dodge,
            NoControl,
            NoAttack,
            Frozen,
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
            if (e_enemyState != EnemyState.Frozen)
            {
                transform.localPosition += S_velocity * f_slowMultiplier * Time.deltaTime;
                CheckCollisions();
            }
        }

        public void HealEnemy(float heal)
        {
            f_currentHealth += heal;
            f_currentHealth = Mathf.Clamp(f_currentHealth, 0, f_baseHealth);
        }

        public void DamageEnemy(float damage)
        {
            f_currentHealth -= damage;
            e_enemyState = EnemyState.Invincible;
            Invoke("NormalizeState", f_invincibleTime);
        }

        public void ApplyBulletElement(BulletEffectInfo bulletEffectInfo, float damage)
        {
            switch (bulletEffectInfo.e_bulletEffect)
            {
                case BulletEffect.None:
                    break;
                case BulletEffect.DamageOverTime:

                    break;
                case BulletEffect.Slow:
                    if (e_enemyState != EnemyState.Frozen)
                        if (f_slowMultiplier >= 0)
                        {
                            f_slowMultiplier -= bulletEffectInfo.f_slowPercent;
                            f_slowMultiplier = Mathf.Clamp(f_slowMultiplier, 0, 1);
                            if (f_slowMultiplier == 0)
                            {
                                e_enemyState = EnemyState.Frozen;
                                S_velocity = Vector3.zero;
                                StartCoroutine(ResetAfterFrozen(bulletEffectInfo.f_effectTime / bulletEffectInfo.f_slowPercent));
                                break;
                            }
                            StartCoroutine(SpeedUpAfterTime(bulletEffectInfo.f_effectTime, bulletEffectInfo.f_slowPercent));
                            break;
                        }
                    break;
                case BulletEffect.Chain:
                    break;
                case BulletEffect.Vampire:
                    HealEnemy(bulletEffectInfo.f_vampirePercent * damage);
                    break;
            }
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
            if(e_enemyState != EnemyState.Frozen)
            {
                S_velocity += velocityToAdd;
            }
        }

        private void NormalizeState()
        {
            e_enemyState = EnemyState.Normal;
        }

        private IEnumerator SpeedUpAfterTime(float effectTime, float increaseAmount)
        {
            yield return new WaitForSeconds(effectTime);
            if (e_enemyState != EnemyState.Frozen)
            {
                f_slowMultiplier += increaseAmount;
            }
            else
            {
                f_slowMultiplier = 0;
            }
        }

        private IEnumerator ResetAfterFrozen(float effectTime)
        {
            yield return new WaitForSeconds(effectTime);
            S_velocity = Vector3.zero;
            f_slowMultiplier = 1;
            NormalizeState();
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