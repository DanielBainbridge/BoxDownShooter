using System.Collections;
using System.Linq;
using UnityEngine;
using Utility;

namespace Gun
{
    public class Gun : MonoBehaviour
    {
        /// <summary>
        /// Reflection of stats inside of gun module
        /// </summary>

        #region GunModuleReflection
        //Trigger Group
        float f_baseDamage;
        float f_fireRate;
        float f_bulletSpeed;
        float f_knockBack;
        GunModule.BulletTraitInfo S_bulletTraitInfo;

        //Clip Group
        float f_reloadSpeed;
        float f_movementPenalty;
        int i_clipSize;
        GunModule.BulletEffectInfo S_bulletEffectInfo;


        //Barrel Group
        GunModule.ShotPattern e_shotPattern;
        float f_bulletSize;
        float f_bulletRange;
        float f_recoil;
        bool b_burstTrue;
        int i_burstCount;
        float f_burstInterval;

        GunModule.ShotPatternInfo S_shotPatternInfo;
        #endregion


        [Rename("Muzzle Transform")] public Transform C_muzzle;

        private Vector3 S_muzzlePosition
        {
            get { return C_muzzle == null ? transform.position : C_muzzle.position; }
        }

        public Transform C_gunHolder;
        public GunModule[] aC_moduleArray = new GunModule[3];

        float f_lastFireTime = 0;
        float f_timeSinceLastFire { get { return Time.time - f_lastFireTime; } }
        float f_timeBetweenBulletShots { get { return 1.0f / f_fireRate; } }
        float f_timeUntilNextFire = 0;

        int i_currentAmmo;

        bool b_isFiring = false;
        float f_fireHoldTime = 0;
        bool b_reloadCancel = false;
        BulletObjectPool C_bulletPool;

        GameObject C_bulletPrefab;

        Bullet.BulletBaseInfo S_bulletInfo { get { return new Bullet.BulletBaseInfo(tag.ToLower().Contains("player"), S_muzzlePosition, C_gunHolder.forward, f_bulletRange, f_baseDamage, f_bulletSpeed); } }

        private void Awake()
        {
            for (int i = 0; i < aC_moduleArray.Count(); i++)
            {
                UpdateGunStats(aC_moduleArray[i]);
            }

            GameObject bulletPool = new GameObject();
            bulletPool.name = "Bullet Pool";
            C_bulletPool = bulletPool.AddComponent<BulletObjectPool>();
            bulletPool.GetComponent<BulletObjectPool>().CreatePool(this);

            i_currentAmmo = i_clipSize;
        }

        private void Update()
        {
            if (b_isFiring)
            {
                f_timeUntilNextFire -= Time.deltaTime;
                f_fireHoldTime += Time.deltaTime;
            }

        }


        public void Fire()
        {

            if (f_timeSinceLastFire < f_timeBetweenBulletShots)
            {
                return;
            }

            if (i_currentAmmo == 0 || i_currentAmmo < i_burstCount)
            {
                Reload();
                return;
            }

            b_isFiring = true;


            while (f_timeUntilNextFire < 0.0f)
            {
                float timeIntoNextFrame = -f_timeUntilNextFire;
                //Spawn Bullet, at muzzle position + (bullet trajectory * bulletspeed) * time into next frame
                if (!b_burstTrue)
                {

                    switch (S_shotPatternInfo.e_shotPattern)
                    {
                        case GunModule.ShotPattern.Straight:
                            FireStraight(timeIntoNextFrame);
                            return;
                        case GunModule.ShotPattern.Multishot:
                            //will need coroutine if you don't do something clever. Think.
                            FireMultiShot(timeIntoNextFrame);
                            return;
                        case GunModule.ShotPattern.Buckshot:
                            FireBuckShot(timeIntoNextFrame);
                            return;
                        case GunModule.ShotPattern.Spray:
                            FireSpray(timeIntoNextFrame);
                            return;
                        case GunModule.ShotPattern.Wave:
                            FireWave(timeIntoNextFrame);
                            return;
                    }
                }

                f_timeUntilNextFire += f_timeBetweenBulletShots;
            }

            f_lastFireTime = Time.time;

            //while time until next bullet is < 0, check time into next frame = -time until next bullet
            //time until next bullet += timeBetween bullets

            //grab bullet from bullet object pool

            //fire amount of bullets from object pool dependent on on gun variables


            //what variables do we need to read before firing,
            // recoil, burst variables, spread variables, fire rate

            //recoil player by recoil distance, add velocity += -aimDirection * distance


            //store bullets that have been shot and then modify them

            //what variables do we need to apply to the bullet
            // size, range, speed, effect + effect parameters, bullet trait, knockback
        }
        public void CancelFire()
        {
            f_fireHoldTime = 0;
            b_isFiring = false;
        }

        //stub
        public void Reload()
        {
            // read clip size and current bullet count and reload time
            // reload 1 at a time,
            //optional cancelleable reload
            i_currentAmmo = i_clipSize;
        }

        public void UpdateGunStats(GunModule gunModule)
        {
            //only update stats that we change

            switch (gunModule.e_moduleType)
            {
                case GunModule.ModuleSection.Trigger:
                    UpdateTriggerStats(gunModule);
                    aC_moduleArray[(int)GunModule.ModuleSection.Trigger] = gunModule;
                    return;
                case GunModule.ModuleSection.Clip:
                    UpdateClipStats(gunModule);
                    aC_moduleArray[(int)GunModule.ModuleSection.Clip] = gunModule;
                    return;
                case GunModule.ModuleSection.Barrel:
                    UpdateBarrelStats(gunModule);
                    aC_moduleArray[(int)GunModule.ModuleSection.Barrel] = gunModule;
                    return;
            }

        }

        /// <summary>
        /// Updates Variables for gun dependent on the gun modules type.
        /// </summary>

        private void UpdateTriggerStats(GunModule gunModule)
        {
            if (gunModule.e_moduleType != GunModule.ModuleSection.Trigger)
            {
                return;
            }

            f_baseDamage = gunModule.f_baseDamage;
            f_fireRate = gunModule.f_fireRate;
            f_bulletSpeed = gunModule.f_bulletSpeed;
            f_knockBack = gunModule.f_knockBack;


            S_bulletTraitInfo = gunModule.S_bulletTraitInformation;

        }
        private void UpdateClipStats(GunModule gunModule)
        {
            if (gunModule.e_moduleType != GunModule.ModuleSection.Clip)
            {
                return;
            }

            f_reloadSpeed = gunModule.f_reloadSpeed;
            f_movementPenalty = gunModule.f_movementPenalty;
            i_clipSize = gunModule.i_clipSize;

            S_bulletEffectInfo = gunModule.S_bulletEffectInformation;

        }
        private void UpdateBarrelStats(GunModule gunModule)
        {
            if (gunModule.e_moduleType != GunModule.ModuleSection.Barrel)
            {
                return;
            }

            f_bulletSize = gunModule.f_bulletSize;
            f_bulletRange = gunModule.f_bulletRange;
            f_recoil = gunModule.f_recoil;
            b_burstTrue = gunModule.b_burstTrue;
            f_burstInterval = gunModule.f_burstInterval;
            i_burstCount = gunModule.i_burstCount;

            S_shotPatternInfo = gunModule.S_shotPatternInformation;

        }


        /// <summary>
        /// Firing Support Functions
        /// </summary>
        private void FireStraight(float timeIntoNextFrame)
        {
            Debug.Log($"Forward: {C_gunHolder.forward}");


            C_bulletPool.GetFirstOpen().FireBullet(S_bulletInfo.S_firingDirection * timeIntoNextFrame, Vector3.zero, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
        }
        private void FireMultiShot(float timeIntoNextFrame)
        {
            for (int i = 0; i < S_shotPatternInfo.i_shotCount; i++)
            {
                C_bulletPool.GetFirstOpen().FireBullet(S_bulletInfo.S_firingDirection * timeIntoNextFrame + (transform.right * (-S_shotPatternInfo.f_multiShotDistance + (i * (2 * S_shotPatternInfo.f_multiShotDistance / S_shotPatternInfo.i_shotCount - 1)))),
                    Vector3.zero, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
            }
        }

        private void FireBuckShot(float timeIntoNextFrame)
        {
            Vector3 fireAngle;
            if (S_shotPatternInfo.b_randomSpread)
            {
                for (int i = 0; i < S_shotPatternInfo.i_shotCount; i++)
                {
                    fireAngle = new Vector3(0, Random.Range(-S_shotPatternInfo.f_maxAngle, S_shotPatternInfo.f_maxAngle), 0);
                    C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection + fireAngle) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
                }
            }
            else
            {
                for (int i = 0; i < S_shotPatternInfo.i_shotCount; i++)
                {
                    fireAngle = new Vector3(0, -S_shotPatternInfo.f_maxAngle + (i * (2 * S_shotPatternInfo.f_maxAngle / (float)(S_shotPatternInfo.i_shotCount - 1))), 0);
                    C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection + fireAngle) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
                }
            }
        }
        private void FireSpray(float timeIntoNextFrame)
        {
            Vector3 fireAngle = new Vector3(0, Random.Range(-S_shotPatternInfo.f_maxAngle, S_shotPatternInfo.f_maxAngle), 0);
            C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection + fireAngle) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
        }
        private void FireWave(float timeIntoNextFrame)
        {
            Vector3 fireAngle = new Vector3(0, ExtraMaths.Map(-1, 1, -S_shotPatternInfo.f_maxAngle, S_shotPatternInfo.f_maxAngle, Mathf.Sin(f_fireHoldTime)), 0);
            C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection + fireAngle) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
        }



        //stub
        public void SwapGunPiece()
        {

        }
        //stub
        public void ResetToBaseStats()
        {

        }

        //stub BURST STUFF
        private IEnumerator FireVolley(int volleyCount, float waitTime)
        {
            int amountOfBulletsFired = 0;
            while (amountOfBulletsFired < volleyCount)
            {

                yield return null;
            }
        }
    }
}
