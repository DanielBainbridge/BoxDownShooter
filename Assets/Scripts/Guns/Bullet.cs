using Enemy;
using UnityEngine;
using Explosion;
using static Gun.GunModule;
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

        Vector3 S_hitDirection
        {
            get { return new Vector3(transform.forward.x, 0, transform.forward.z); }
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

        private void DoBaseHit(Combatant combatant)
        {
            combatant.Damage(S_baseInformation.f_damage);
            combatant.AddVelocity(S_hitDirection * S_baseInformation.f_knockBack);
            combatant.ApplyBulletElement(S_bulletEffect, S_baseInformation.f_damage);
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



            Combatant combatant = objectHit.GetComponent<Combatant>();

            if (combatant == null)
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



            //do boolean calcs
            bool isPlayer = combatant.CompareTag("Player");
            bool isDodging = (combatant.e_combatState == Combatant.CombatState.Dodge);
            bool isInvincible = (combatant.e_combatState == Combatant.CombatState.Invincible);
            bool shouldHit = (!(isPlayer && !S_baseInformation.b_playerOwned) && (!isDodging && !isInvincible)) ||
                (!(!isPlayer && S_baseInformation.b_playerOwned) && (isDodging && !isInvincible));


            switch (S_bulletTrait.e_bulletTrait)
            {
                case BulletTrait.Standard:
                    if (shouldHit)
                    {
                        //do damage
                        DoBaseHit(combatant);
                        C_poolOwner.MoveToOpen(this);
                        return true;
                    }
                    return false;                    
                case BulletTrait.Pierce:
                    if (shouldHit)
                    {
                        //do damage
                        DoBaseHit(combatant);
                        i_bulletPiercedCount += 1;

                        if (i_bulletPiercedCount == S_bulletTrait.i_pierceCount)
                        {
                            C_poolOwner.MoveToOpen(this);
                            return true;
                        }

                        if (S_bulletEffect.e_bulletEffect == BulletEffect.Chain && S_bulletEffect.i_ricochetCount >= i_ricochetCount)
                        {
                            if (isPlayer)
                            {
                                i_ricochetCount += 1;
                                transform.rotation = Quaternion.Euler(new Vector3(0, ExtraMaths.FloatRandom(0, 360), 0));
                            }
                            else
                            {
                                i_ricochetCount += 1;
                                Transform newTarget = FindRicochetTarget(combatant.transform);
                                if (newTarget != null)
                                {
                                    transform.LookAt(newTarget);
                                    transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
                                }
                            }
                        }
                    }
                    return false;
                case BulletTrait.Explosive:
                    if (shouldHit)
                    {
                        DoBaseHit(combatant);
                        //create explosion with explosion size for amount of time and then
                        ExplosionGenerator.MakeExplosion(transform.position, S_bulletTrait.C_explosionPrefab, S_bulletTrait.f_explosionSize, S_bulletTrait.f_explosionDamage, S_bulletTrait.f_explosionKnockbackDistance, S_bulletTrait.f_explosionLifeTime);
                        C_poolOwner.MoveToOpen(this);
                        return true;
                    }
                    return false;
                case BulletTrait.Homing:
                    if (shouldHit)
                    {
                        DoBaseHit(combatant);
                        C_homingTarget = null;
                        C_poolOwner.MoveToOpen(this);
                        return true;
                    }
                    return false;
            }
            return false;

        }
    }
}
