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

        [Rename("Gun Holder")] public Transform C_gunHolder;
        [Rename("Gun Modules")]public GunModule[] aC_moduleArray = new GunModule[3];

        float f_lastFireTime = 0;
        float f_timeSinceLastFire { get { return Time.time - f_lastFireTime; } }
        float f_timeBetweenBulletShots { get { return 1.0f / f_fireRate; } }
        float f_timeUntilNextFire = 0;

        int i_currentAmmo;

        bool b_isFiring = false;
        float f_fireHoldTime = 0;
        bool b_reloadCancel = false;
        bool b_reloading = false;
        BulletObjectPool C_bulletPool;

        GameObject C_bulletPrefab;

        Bullet.BulletBaseInfo S_bulletInfo { get { return new Bullet.BulletBaseInfo(tag.ToLower().Contains("player"), S_muzzlePosition, C_gunHolder.forward, f_bulletRange, f_baseDamage, f_bulletSpeed, f_bulletSize); } }

        private void Awake()
        {

            GameObject bulletPool = new GameObject();
            bulletPool.name = "Bullet Pool";
            C_bulletPool = bulletPool.AddComponent<BulletObjectPool>();
            bulletPool.GetComponent<BulletObjectPool>().CreatePool(this);

            for (int i = 0; i < aC_moduleArray.Count(); i++)
            {
                UpdateGunStats(aC_moduleArray[i]);
            }

            i_currentAmmo = i_clipSize;
        }

        private void Update()
        {
            if (b_isFiring && !b_reloading)
            {
                f_timeUntilNextFire -= Time.deltaTime;
                f_fireHoldTime += Time.deltaTime;
                Fire();
            }
        }

        //temp
        private void FixedUpdate()
        {
            for (int i = 0; i < aC_moduleArray.Count(); i++)
            {
                UpdateGunStats(aC_moduleArray[i]);
            }
        }


        public void StartFire()
        {
            b_isFiring = true;
        }

        private void Fire()
        {
            if (f_timeSinceLastFire < f_timeBetweenBulletShots || b_reloading)
            {
                return;
            }
            if (i_currentAmmo <= 0 || i_currentAmmo < i_burstCount)
            {
                StartCoroutine(ReloadAfterTime());
                return;
            }

            int timesFiredThisFrame = 0;

            while (f_timeUntilNextFire < 0.0f)
            {
                float timeIntoNextFrame = -f_timeUntilNextFire;
                //Spawn Bullet, at muzzle position + (bullet trajectory * bulletspeed) * time into next frame
                if (!b_burstTrue)
                {
                    // TO DO: FRAME STUFF
                    switch (S_shotPatternInfo.e_shotPattern)
                    {
                        case GunModule.ShotPattern.Straight:
                            FireStraight(timeIntoNextFrame);
                            break;
                        case GunModule.ShotPattern.Multishot:
                            //will need coroutine if you don't do something clever. Think.
                            FireMultiShot(timeIntoNextFrame);
                            break;
                        case GunModule.ShotPattern.Buckshot:
                            FireBuckShot(timeIntoNextFrame);
                            break;
                        case GunModule.ShotPattern.Spray:
                            FireSpray(timeIntoNextFrame);
                            break;
                        case GunModule.ShotPattern.Wave:
                            FireWave(timeIntoNextFrame);
                            break;
                    }
                }
                timesFiredThisFrame += 1;

                f_timeUntilNextFire += f_timeBetweenBulletShots;
            }

            if (C_gunHolder.GetComponent<PlayerController>())
            {
                C_gunHolder.GetComponent<PlayerController>().AddVelocityToPlayer(-C_gunHolder.forward * f_recoil);
            }

            f_lastFireTime = Time.time;
            i_currentAmmo -= timesFiredThisFrame;

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

        /// <summary>
        /// Updates Variables for gun dependent on the gun modules type.
        /// </summary>
        public void UpdateGunStats(GunModule gunModule)
        {
            //only update stats that we change

            switch (gunModule.e_moduleType)
            {
                case GunModule.ModuleSection.Trigger:
                    UpdateTriggerStats(gunModule);
                    aC_moduleArray[(int)GunModule.ModuleSection.Trigger] = gunModule;
                    break;
                case GunModule.ModuleSection.Clip:
                    UpdateClipStats(gunModule);
                    aC_moduleArray[(int)GunModule.ModuleSection.Clip] = gunModule;
                    break;
                case GunModule.ModuleSection.Barrel:
                    UpdateBarrelStats(gunModule);
                    aC_moduleArray[(int)GunModule.ModuleSection.Barrel] = gunModule;
                    break;
            }
            C_bulletPool.ResizePool(this);
        }        
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
            C_bulletPool.GetFirstOpen().FireBullet(S_bulletInfo.S_firingDirection * timeIntoNextFrame, Vector3.zero, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
        }
        private void FireMultiShot(float timeIntoNextFrame)
        {
            for (int i = 0; i < S_shotPatternInfo.i_shotCount; i++)
            {
                C_bulletPool.GetFirstOpen().FireBullet(S_bulletInfo.S_firingDirection * timeIntoNextFrame + (transform.right * (-S_shotPatternInfo.f_multiShotDistance * (S_shotPatternInfo.i_shotCount - 1)) / 2.0f) + (transform.right * (i * (S_shotPatternInfo.f_multiShotDistance))),
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
                    fireAngle = new Vector3(0, ExtraMaths.FloatRandom(-S_shotPatternInfo.f_maxAngle, S_shotPatternInfo.f_maxAngle), 0);
                    C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
                }
            }
            else
            {
                for (int i = 0; i < S_shotPatternInfo.i_shotCount; i++)
                {
                    fireAngle = new Vector3(0, -S_shotPatternInfo.f_maxAngle + (i * (2 * S_shotPatternInfo.f_maxAngle / (float)(S_shotPatternInfo.i_shotCount - 1))), 0);
                    C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
                }
            }
        }
        private void FireSpray(float timeIntoNextFrame)
        {
            Vector3 fireAngle = new Vector3(0, Random.Range(-S_shotPatternInfo.f_maxAngle, S_shotPatternInfo.f_maxAngle), 0);
            C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
        }
        private void FireWave(float timeIntoNextFrame)
        {
            Vector3 fireAngle = new Vector3(0, ExtraMaths.Map(-1, 1, -S_shotPatternInfo.f_maxAngle, S_shotPatternInfo.f_maxAngle, Mathf.Sin(f_fireHoldTime)), 0);
            C_bulletPool.GetFirstOpen().FireBullet((S_bulletInfo.S_firingDirection) * timeIntoNextFrame, fireAngle, S_bulletInfo, S_bulletTraitInfo, S_bulletEffectInfo);
        }

        //stub
        public void SwapGunPiece(GunModule newModule)
        {
            switch (newModule.e_moduleType)
            {
                case GunModule.ModuleSection.Trigger:
                    aC_moduleArray[0] = newModule;
                    break;
                case GunModule.ModuleSection.Clip:
                    aC_moduleArray[1] = newModule;
                    break;
                case GunModule.ModuleSection.Barrel:
                    aC_moduleArray[2] = newModule;
                    break;
            }
            UpdateGunStats(newModule);
        }
        
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
        //reload all at once
        private IEnumerator ReloadAfterTime()
        {
            b_reloading = true;
            yield return new WaitForSeconds(f_reloadSpeed);
            Reload();
            b_reloading = false;
        }
        //reload one bullet at a time
        private IEnumerator ReloadOverTime()
        {
            b_reloading = true;
            float reloadRate = i_clipSize / f_reloadSpeed;
            while(i_currentAmmo != i_clipSize)
            {
                yield return new WaitForSeconds(reloadRate);
                i_currentAmmo++;
            }
            b_reloading = false;
        }
    }
}
