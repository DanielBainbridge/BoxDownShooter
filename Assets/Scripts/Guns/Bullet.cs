using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using static Gun.GunModule;

namespace Gun
{
    public class Bullet : MonoBehaviour
    {
        public struct BulletBaseInfo
        {
            [HideInInspector] public bool b_playerOwned;
            [HideInInspector] public Vector3 S_firingOrigin;
            [HideInInspector] public Vector3 S_firingDirection;
            [HideInInspector] public float f_range;
            [HideInInspector] public float f_damage;
            [HideInInspector] public float f_speed;
            [HideInInspector] public float f_size;

            public BulletBaseInfo(bool playerOwned, Vector3 origin, Vector3 direction, float range, float damage, float speed, float size)
            {
                b_playerOwned = playerOwned;
                S_firingOrigin = origin;
                S_firingDirection = direction;
                f_range = range;
                f_damage = damage;
                f_speed = speed;
                f_size = size;
            }
        }


        [HideInInspector] public GameObject C_prefab;
        [HideInInspector] public BulletObjectPool C_poolOwner;
        [HideInInspector] private BulletBaseInfo S_baseInformation;
        private Vector3 S_previousPosition;

        [HideInInspector] private float f_bulletAliveTime;
        private int i_bulletPiercedCount = 0;
        private int i_ricochetCount = 0;
        private int i_enemiesChained = 0;


        BulletEffectInfo S_bulletEffect;
        BulletTraitInfo S_bulletTrait;



        void Update()
        {
            if (CheckHit())
            {
                return;
            }
            S_previousPosition = transform.position;



            if (S_baseInformation.f_speed == 0)
            {
                if (f_bulletAliveTime > S_baseInformation.f_range)
                {
                    C_poolOwner.MoveToOpen(this);
                    return;
                }
            }

            //check that it hasn't gone past its range, set inactive
            if (Vector3.Distance(S_baseInformation.S_firingOrigin, transform.position) > S_baseInformation.f_range)
            {
                C_poolOwner.MoveToOpen(this);
                return;
            }



            //TO DO, update differently based on bullet type
            switch (S_bulletTrait.e_bulletTrait)
            {
                case BulletTrait.Standard:
                    // move in direction by speed, 
                    transform.position += transform.forward * S_baseInformation.f_speed * Time.deltaTime;
                    break;
                case BulletTrait.Pierce:
                    transform.position += transform.forward * S_baseInformation.f_speed * Time.deltaTime;
                    break;
                case BulletTrait.Explosive:
                    transform.position += transform.forward * S_baseInformation.f_speed * Time.deltaTime;
                    break;
                case BulletTrait.Homing:
                    break;

            }
            f_bulletAliveTime += Time.deltaTime;

        }

        public void FireBullet(Vector3 originOffset, Vector3 directionOffset, BulletBaseInfo bulletInfo, GunModule.BulletTraitInfo bulletTrait, GunModule.BulletEffectInfo bulletEffect)
        {
            f_bulletAliveTime = 0;
            i_bulletPiercedCount = 0;
            i_ricochetCount = 0;
            i_enemiesChained = 0;

            //bullet effect colour/ particles bullet trait mesh **stubbed**
            //UpdateBulletGraphics()

            //move bullet to closed list
            C_poolOwner.MoveToClosed(this);

            S_baseInformation = bulletInfo;

            transform.localScale = new Vector3(S_baseInformation.f_size, S_baseInformation.f_size, S_baseInformation.f_size);

            S_previousPosition = transform.position = bulletInfo.S_firingOrigin + originOffset;
            transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(-bulletInfo.S_firingDirection.z, bulletInfo.S_firingDirection.x) * Mathf.Rad2Deg + 90, 0) + directionOffset);

            S_bulletEffect = bulletEffect;
            S_bulletTrait = bulletTrait;
        }

        public void BulletChangeDirection(Vector3 direction)
        {
            S_baseInformation.S_firingDirection = direction;
        }
        public void BulletChangeOrigin(Vector3 origin)
        {
            S_baseInformation.S_firingOrigin = origin;
        }

        //stub need bullet stuff in
        public void UpdateBulletGraphics()
        {
            Vector4 colour;
            GameObject meshPrefab;
            GameObject particlePrefab;
        }
        private bool CheckHit()
        {

            if (Physics.SphereCast(transform.position - (transform.forward * S_baseInformation.f_size * 1.01f), S_baseInformation.f_size * 2, (transform.position - S_previousPosition).normalized, out RaycastHit hitInfo, Vector3.Distance(S_previousPosition, transform.position)))
            {
                OnHit(hitInfo);
                return true;
            }
            return false;
        }
        public void OnHit(RaycastHit hit)
        {
            Transform objectHit = hit.transform;

            Enemy.EnemyBase enemyBase = objectHit.GetComponent<Enemy.EnemyBase>();
            PlayerController playerController = objectHit.GetComponent<PlayerController>();

            bool isPlayer = playerController == null ? false : true;
            bool isEnemy = enemyBase == null ? false : true;

            if (!isPlayer && !isEnemy)
            {
                C_poolOwner.MoveToOpen(this);
                return;
            }


            switch (S_bulletTrait.e_bulletTrait)
            {
                case BulletTrait.Standard:
                    if (isPlayer && !S_baseInformation.b_playerOwned && (playerController.e_playerState != PlayerController.PlayerState.Invincible || playerController.e_playerState != PlayerController.PlayerState.Dodge))
                    {
                        //do damage
                        playerController.DamagePlayer(S_baseInformation.f_damage);
                        C_poolOwner.MoveToOpen(this);
                    }
                    else if (isEnemy)
                    {
                        enemyBase.TakeDamage((int)S_baseInformation.f_damage);
                        C_poolOwner.MoveToOpen(this);
                    }
                    break;
                case BulletTrait.Pierce:
                    if (isPlayer && !S_baseInformation.b_playerOwned && (playerController.e_playerState != PlayerController.PlayerState.Invincible || playerController.e_playerState != PlayerController.PlayerState.Dodge))
                    {
                        //do damage
                        playerController.DamagePlayer(S_baseInformation.f_damage);
                        i_bulletPiercedCount += 1;

                        if (i_bulletPiercedCount == S_bulletTrait.i_pierceCount)
                        {
                            C_poolOwner.MoveToOpen(this);
                        }
                    }
                    break;
                case BulletTrait.Explosive:
                    if (isPlayer && !S_baseInformation.b_playerOwned && (playerController.e_playerState != PlayerController.PlayerState.Invincible || playerController.e_playerState != PlayerController.PlayerState.Dodge))
                    {
                        playerController.DamagePlayer(S_baseInformation.f_damage);
                        //create explosion with explosion size for amount of time and then 

                    }
                    break;
                case BulletTrait.Homing:
                    if (isPlayer && !S_baseInformation.b_playerOwned)
                    {

                    }
                    break;
            }

            switch (S_bulletEffect.e_bulletEffect)
            {
                case BulletEffect.None:
                    break;
                case BulletEffect.DamageOverTime:
                    if (isPlayer && !S_baseInformation.b_playerOwned)
                    {

                    }
                    break;
                case BulletEffect.Slow:
                    if (isPlayer && !S_baseInformation.b_playerOwned)
                    {

                    }
                    break;
                case BulletEffect.Chain:
                    if (isPlayer && !S_baseInformation.b_playerOwned)
                    {

                    }
                    break;
                case BulletEffect.Vampire:
                    if (isPlayer && !S_baseInformation.b_playerOwned)
                    {

                    }
                    break;
            }
        }
    }
}
