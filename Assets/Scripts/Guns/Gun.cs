using JetBrains.Annotations;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Pool;
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
        //shot pattern dependent
        int i_burstCount;
        float f_burstInterval;
        bool b_randomSpread;
        float f_spreadMaxAngle;
        int i_spreadCount;
        #endregion

        Vector3 S_muzzlePosition;

        public Transform C_gunHolder;
        public GunModule[] aC_moduleArray = new GunModule[3];

        float f_lastFireTime = 0;
        float f_timeSinceLastFire { get { return Time.time - f_lastFireTime; } }
        float f_timeBetweenBulletShots { get { return 1.0f / f_fireRate; } }
        float f_timeUntilNextFire = 0;

        int i_effectiveCurrentAmmo;
        int i_actualCurrentAmmo;

        bool b_isFiring = false;
        bool b_reloadCancel = false;
        BulletObjectPool C_bulletPool;

        GameObject C_bulletPrefab;

        Bullet.BulletBaseInfo S_bulletInfo { get { return new Bullet.BulletBaseInfo(tag.ToLower().Contains("player"), S_muzzlePosition, transform.forward, f_bulletRange, f_baseDamage, f_bulletSpeed); } }


        public void Fire()
        {

            if (f_timeSinceLastFire < f_fireRate)
            {
                return;
            }

            if (i_effectiveCurrentAmmo == 0 || i_effectiveCurrentAmmo < i_burstCount)
            {
                Reload();
            }

            b_isFiring = true;

            while (f_timeUntilNextFire < 0.0f)
            {
                float timeIntoNextFrame = -f_timeUntilNextFire;
                //Spawn Bullet, at muzzle position + (bullet trajectory * bulletspeed) * time into next frame

                switch (e_shotPattern)
                {
                    case GunModule.ShotPattern.Straight:
                        C_bulletPool.GetFirstOpen().FireBullet(Vector3.zero, Vector3.zero, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
                        return;
                    case GunModule.ShotPattern.Burst:
                        //will need coroutine if you don't do something clever. Think.
                        for (int i = 0; i < i_burstCount; i++)
                        {

                        }
                        return;
                    case GunModule.ShotPattern.Spread:

                        if (b_randomSpread)
                        {
                            for (int i = 0; i < i_spreadCount; i++)
                            {
                                C_bulletPool.GetFirstOpen().FireBullet(Vector3.zero, Vector3.zero, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < i_spreadCount; i++) 
                            {
                                C_bulletPool.GetFirstOpen().FireBullet(Vector3.zero, Vector3.zero, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
                            }

                        }

                        return;
                    case GunModule.ShotPattern.Spray:
                        return;
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

        private void Update()
        {
            if (b_isFiring)
            {
                f_timeUntilNextFire -= Time.deltaTime;
            }

        }



        public void Reload()
        {
            // read clip size and current bullet count and reload time
            // reload 1 at a time,
            //optional cancelleable reload

        }

        public void UpdateGunStats(GunModule gunModule)
        {
            //only update stats that we change
            for (int i = 0; i < aC_moduleArray.Length; i++)
            {
                switch (aC_moduleArray[i].e_moduleType)
                {
                    case GunModule.ModuleSection.Trigger:
                        UpdateTriggerStats(gunModule);
                        return;
                    case GunModule.ModuleSection.Clip:
                        UpdateClipStats(gunModule);
                        return;
                    case GunModule.ModuleSection.Barrel:
                        UpdateBarrelStats(gunModule);
                        return;
                }
            }
        }

        /// <summary>
        /// Updates Variables for gun dependent on the gun modules type.
        /// 
        /// TODO Reset values to the 
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

            e_shotPattern = gunModule.e_shotPattern;

            i_burstCount = gunModule.i_burstCount;
            f_burstInterval = gunModule.f_burstInterval;
            b_randomSpread = gunModule.b_randomSpread;
            f_spreadMaxAngle = gunModule.f_spreadMaxAngle;
            i_spreadCount = gunModule.i_spreadCount;


        }


        public void SwapGunPiece()
        {

        }

        public void ResetToBaseStats()
        {

        }

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
