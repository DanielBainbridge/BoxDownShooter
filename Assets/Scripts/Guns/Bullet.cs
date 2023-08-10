using Enemy;
using UnityEngine;
using Explosion;
using static Gun.GunModule;
using UnityEngine.UIElements;
using Utility;

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
            [HideInInspector] public float f_knockBack;

            public BulletBaseInfo(bool playerOwned, Vector3 origin, Vector3 direction, float range, float damage, float speed, float size, float knockBack)
            {
                b_playerOwned = playerOwned;
                S_firingOrigin = origin;
                S_firingDirection = direction;
                f_range = range;
                f_damage = damage;
                f_speed = speed;
                f_size = size;
                f_knockBack = knockBack;
            }
        }


        [HideInInspector] public GameObject C_prefab;
        [HideInInspector] public BulletObjectPool C_poolOwner;
        [HideInInspector] private BulletBaseInfo S_baseInformation;
        private Vector3 S_previousPosition;
        private float f_rotationalAcceleration;
        private float f_rotationalVelocity;
        private float f_desiredRotationAngle
        {
            get
            {
                Vector3 direction = (C_homingTarget.position - transform.position).normalized;
                return (-Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg) + 90;
            }
        }
        private float f_currentRotationAngle
        {
            get
            {
                Vector3 direction = transform.forward;
                return (-Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg) + 90;
            }
        }

        private float f_bulletAliveTime;
        private float f_distanceTravelled;
        private int i_bulletPiercedCount = 0;
        private int i_ricochetCount = 0;
        private int i_targetsChained = 0;
        private Transform C_homingTarget;


        BulletEffectInfo S_bulletEffect;
        BulletTraitInfo S_bulletTrait;



        void Update()
        {
            if (CheckHit())
            {
                return;
            }
            S_previousPosition = transform.position;


            //check that it hasn't gone past its range, set inactive
            if (f_bulletAliveTime > S_baseInformation.f_range || f_distanceTravelled > S_baseInformation.f_range)
            {
                C_poolOwner.MoveToOpen(this);
                return;
            }

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
                    //Dot Product of thing multiplied by rad2deg multiplied by homing intensity
                    CheckHomingTarget();
                    if (f_bulletAliveTime > S_bulletTrait.f_homingDelayTime && C_homingTarget != null)
                    {

                        float angleToTarget = f_desiredRotationAngle - f_currentRotationAngle;
                        if (angleToTarget < -180)
                        {
                            angleToTarget += 360;
                        }
                        else if (angleToTarget > 180)
                        {
                            angleToTarget -= 360;
                        }

                        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + angleToTarget * S_bulletTrait.f_homingStrength, 0);
                    }
                    transform.position += transform.forward * S_baseInformation.f_speed * Time.deltaTime;
                    break;

            }
            f_bulletAliveTime += Time.deltaTime;
            f_distanceTravelled += Vector3.Distance(transform.position, S_previousPosition);
        }

        public void FireBullet(Vector3 originOffset, Vector3 directionOffset, BulletBaseInfo bulletInfo, GunModule.BulletTraitInfo bulletTrait, GunModule.BulletEffectInfo bulletEffect)
        {
            f_bulletAliveTime = 0;
            f_distanceTravelled = 0;
            i_bulletPiercedCount = 0;
            i_ricochetCount = 0;
            i_targetsChained = 0;

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
            if (S_bulletTrait.e_bulletTrait == BulletTrait.Homing)
            {
                FindHomingTarget();
            }
        }
        void FindHomingTarget()
        {
            if (S_baseInformation.b_playerOwned)
            {
                float closestEnemyRotation = float.MaxValue;
                int closestEnemy = int.MaxValue;
                EnemyBase[] enemiesOnScreen = FindObjectsOfType<EnemyBase>();

                for (int i = 0; i < enemiesOnScreen.Length; i++)
                {
                    Vector3 toEnemy = (transform.position - enemiesOnScreen[i].transform.position).normalized;
                    float angleToTarget = Vector3.Angle(toEnemy, transform.forward);
                    angleToTarget = -angleToTarget;

                    if (angleToTarget < closestEnemyRotation)
                    {
                        closestEnemyRotation = angleToTarget;
                        closestEnemy = i;
                    }
                }

                if (closestEnemy == int.MaxValue)
                {
                    return;
                }
                C_homingTarget = enemiesOnScreen[closestEnemy].transform;
            }
            else
            {
                C_homingTarget = FindObjectOfType<PlayerController>().transform;
            }
        }

        private Transform FindRicochetTarget(Transform lastHit)
        {
            if (S_baseInformation.b_playerOwned)
            {
                float closestEnemyDistance = float.MaxValue;
                int closestEnemy = int.MaxValue;
                EnemyBase[] enemiesOnScreen = FindObjectsOfType<EnemyBase>();

                for (int i = 0; i < enemiesOnScreen.Length; i++)
                {
                    if (enemiesOnScreen[i].transform == lastHit)
                    {
                        continue;
                    }
                    Vector3 toEnemy = (transform.position - enemiesOnScreen[i].transform.position);

                    if (toEnemy.magnitude < closestEnemyDistance)
                    {
                        closestEnemyDistance = toEnemy.magnitude;
                        closestEnemy = i;
                    }
                }

                if (closestEnemy == int.MaxValue)
                {
                    return null;
                }
                return enemiesOnScreen[closestEnemy].transform;
            }
            else
            {
                return FindObjectOfType<PlayerController>().transform;
            }
        }

        void CheckHomingTarget()
        {
            if (C_homingTarget != null)
            {
                if (C_homingTarget.gameObject.activeInHierarchy)
                {
                    return;
                }
            }
            C_homingTarget = null;

            if (S_baseInformation.b_playerOwned)
            {
                FindHomingTarget();
            }
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

        // bool returned early outs of update, if a bullet is destroyed return true else false
        private bool CheckHit()
        {
            Collider[] collisions = Physics.OverlapCapsule(S_previousPosition, transform.position, S_baseInformation.f_size, ~LayerMask.GetMask("Bullet"));
            if (collisions.Length > 0)
            {
                return OnHit(collisions[0].transform);
            }
            return false;
        }
        // bool returned early outs of update, if a bullet is destroyed return true else false
        public bool OnHit(Transform objectHit)
        {

            Vector3 hitDirection = (objectHit.position - transform.position);
            hitDirection = new Vector3(transform.forward.x, 0, transform.forward.z);

            Enemy.EnemyBase enemyBase = objectHit.GetComponent<EnemyBase>();
            PlayerController playerController = objectHit.GetComponent<PlayerController>();

            bool isPlayer = playerController == null ? false : true;
            bool isEnemy = enemyBase == null ? false : true;

            if (!isPlayer && !isEnemy)
            {
                if (S_bulletEffect.e_bulletEffect == BulletEffect.Chain && S_bulletEffect.i_ricochetCount >= i_ricochetCount)
                {
                    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, S_baseInformation.f_size, ~LayerMask.GetMask("Bullet")))
                    {
                        transform.forward = Vector3.Reflect(transform.forward, hit.normal);
                        i_ricochetCount += 1;
                    }
                    return false;
                }
                else if (S_bulletTrait.e_bulletTrait == BulletTrait.Explosive)
                {
                    ExplosionGenerator.MakeExplosion(transform.position, S_bulletTrait.C_explosionPrefab, S_bulletTrait.f_explosionSize, S_bulletTrait.f_explosionDamage, S_bulletTrait.f_explosionKnockbackDistance, S_bulletTrait.f_explosionLifeTime);
                }
                C_poolOwner.MoveToOpen(this);
                return true;
            }
            switch (S_bulletTrait.e_bulletTrait)
            {
                case BulletTrait.Standard:
                    if (isPlayer && !S_baseInformation.b_playerOwned && (playerController.e_playerState != PlayerController.PlayerState.Invincible && playerController.e_playerState != PlayerController.PlayerState.Dodge))
                    {
                        //do damage
                        playerController.DamagePlayer(S_baseInformation.f_damage);
                        playerController.AddVelocityToPlayer(hitDirection * S_baseInformation.f_knockBack);
                        playerController.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                    }
                    else if (isEnemy && S_baseInformation.b_playerOwned && (enemyBase.e_enemyState != EnemyBase.EnemyState.Invincible && enemyBase.e_enemyState != EnemyBase.EnemyState.Dodge))
                    {
                        enemyBase.DamageEnemy((int)S_baseInformation.f_damage);
                        enemyBase.AddVelocityToEnemy(hitDirection * S_baseInformation.f_knockBack);
                        enemyBase.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                    }
                    C_poolOwner.MoveToOpen(this);
                    return true;
                    break;
                case BulletTrait.Pierce:
                    if (isPlayer && !S_baseInformation.b_playerOwned && (playerController.e_playerState != PlayerController.PlayerState.Invincible && playerController.e_playerState != PlayerController.PlayerState.Dodge))
                    {
                        //do damage
                        playerController.DamagePlayer(S_baseInformation.f_damage);
                        playerController.AddVelocityToPlayer(hitDirection * S_baseInformation.f_knockBack);
                        playerController.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                        i_bulletPiercedCount += 1;

                        if (S_bulletEffect.e_bulletEffect == BulletEffect.Chain && S_bulletEffect.i_ricochetCount >= i_ricochetCount)
                        {
                            i_ricochetCount += 1;
                            transform.rotation = Quaternion.Euler(new Vector3(0, ExtraMaths.FloatRandom(0, 360), 0));
                        }
                        return false;
                    }
                    else if (isEnemy && S_baseInformation.b_playerOwned && (enemyBase.e_enemyState != EnemyBase.EnemyState.Invincible && enemyBase.e_enemyState != EnemyBase.EnemyState.Dodge))
                    {
                        enemyBase.DamageEnemy(S_baseInformation.f_damage);
                        enemyBase.AddVelocityToEnemy(hitDirection * S_baseInformation.f_knockBack);
                        enemyBase.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                        i_bulletPiercedCount += 1;


                        if (i_bulletPiercedCount == S_bulletTrait.i_pierceCount)
                        {
                            C_poolOwner.MoveToOpen(this);
                            return true;
                        }
                        if (S_bulletEffect.e_bulletEffect == BulletEffect.Chain && S_bulletEffect.i_ricochetCount >= i_ricochetCount)
                        {
                            i_ricochetCount += 1;
                            Transform newTarget = FindRicochetTarget(enemyBase.transform);
                            if(newTarget != null)
                            {
                                transform.LookAt(newTarget);
                            }
                            return false;
                        }
                        C_poolOwner.MoveToOpen(this);
                        return true;
                    }
                    break;
                case BulletTrait.Explosive:
                    if (isPlayer && !S_baseInformation.b_playerOwned && (playerController.e_playerState != PlayerController.PlayerState.Invincible && playerController.e_playerState != PlayerController.PlayerState.Dodge))
                    {
                        playerController.DamagePlayer(S_baseInformation.f_damage);
                        playerController.AddVelocityToPlayer(hitDirection * S_baseInformation.f_knockBack);
                        playerController.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                    }
                    else if (isEnemy && S_baseInformation.b_playerOwned && (enemyBase.e_enemyState != EnemyBase.EnemyState.Invincible && enemyBase.e_enemyState != EnemyBase.EnemyState.Dodge))
                    {
                        enemyBase.DamageEnemy(S_baseInformation.f_damage);
                        enemyBase.AddVelocityToEnemy(hitDirection * S_baseInformation.f_knockBack);
                        enemyBase.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                    }
                    //create explosion with explosion size for amount of time and then
                    C_poolOwner.MoveToOpen(this);
                    ExplosionGenerator.MakeExplosion(transform.position, S_bulletTrait.C_explosionPrefab, S_bulletTrait.f_explosionSize, S_bulletTrait.f_explosionDamage, S_bulletTrait.f_explosionKnockbackDistance, S_bulletTrait.f_explosionLifeTime);
                    return true;
                    break;
                case BulletTrait.Homing:
                    if (isPlayer && !S_baseInformation.b_playerOwned && (playerController.e_playerState != PlayerController.PlayerState.Invincible && playerController.e_playerState != PlayerController.PlayerState.Dodge))
                    {
                        playerController.DamagePlayer(S_baseInformation.f_damage);
                        playerController.AddVelocityToPlayer(hitDirection * S_baseInformation.f_knockBack);
                        playerController.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                        C_homingTarget = null;
                        C_poolOwner.MoveToOpen(this);
                        return true;
                    }
                    else if (isEnemy && S_baseInformation.b_playerOwned && (enemyBase.e_enemyState != EnemyBase.EnemyState.Invincible && enemyBase.e_enemyState != EnemyBase.EnemyState.Dodge))
                    {
                        enemyBase.DamageEnemy((int)S_baseInformation.f_damage);
                        enemyBase.AddVelocityToEnemy(hitDirection * S_baseInformation.f_knockBack);
                        enemyBase.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
                        C_poolOwner.MoveToOpen(this);
                        return true;
                    }
                    return false;
                    break;
            }
            return false;

        }
    }
}
