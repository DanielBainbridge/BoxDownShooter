using UnityEngine;
using Utility;

namespace Gun
{
    public class Gun : MonoBehaviour
    {
        //reflection of stats inside of gun module


        //Trigger Group
        float f_baseDamage;
        float f_fireRate;
        float f_bulletSpeed;
        float f_knockBack;
        GunModule.BulletTrait e_bulletTrait;
        //bullet trait dependent
        int i_pierceCount;
        float f_explosionSize;
        GameObject C_explosionPrefab;
        float f_explosionKnockbackDistance;
        float f_explosionLingerTime;
        float f_homingStrength;
        float f_homingDelayTime;



        //Clip Group
        float f_reloadSpeed;
        float f_movementPenalty;
        int i_clipSize;
        GunModule.BulletEffect e_bulletEffects;
        //bullet effect dependent
        float f_effectTime;
        float f_tickDamage;
        int i_amountOfTicks;
        float f_slowPercent;
        int i_chainCount;
        float f_chainLength;
        float f_vampirePercent;


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

        Vector3 muzzlePosition;

        public Transform C_gunHolder;
        GunModule[] aC_moduleArray = new GunModule[3];

        float f_lastFireTime = 0;
        float f_timeSinceLastFire
        {
            get { return Time.time - f_lastFireTime; }
        }
        float f_timeBetweenBulletShots
        {
            get { return 1.0f / f_fireRate; }
        }
        float f_timeUntilNextFire = 0;

        bool b_isFiring = false;


        public void Fire()
        {

            if(f_timeSinceLastFire < f_fireRate)
            {
                return;
            }
            b_isFiring = true;

            while (f_timeUntilNextFire < 0.0f)
            {
                float timeIntoNextFrame = -f_timeUntilNextFire;
                //Spawn Bullet, at muzzle position + (bullet trajectory * bulletspeed) * time into next frame
                f_timeUntilNextFire += f_timeBetweenBulletShots;
                
            }


            
            //timebetweenbullets is 1/fire rate

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
            f_lastFireTime = Time.time;
        }

        private void Update()
        {
            if (b_isFiring)
            {
                f_timeUntilNextFire -= Time.deltaTime;
            }

        }

        public void ShootBullet(Vector3 startPosition, Vector3 direction, float speed)
        {

        }


        public void Reload()
        {
            // read clip size and current bullet count and reload time
            // reload 1 at a time,
            //optional cancelleale reload

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


            e_bulletTrait = gunModule.e_bulletTrait;


            i_pierceCount = gunModule.i_pierceCount;
            f_explosionSize = gunModule.f_explosionSize;
            C_explosionPrefab = gunModule.C_explosionPrefab;
            f_explosionKnockbackDistance = gunModule.f_explosionKnockbackDistance;
            f_explosionLingerTime = gunModule.f_explosionLingerTime;
            f_homingStrength = gunModule.f_homingStrength;
            f_homingDelayTime = gunModule.f_homingDelayTime;


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

            e_bulletEffects = gunModule.e_bulletEffects;

            f_effectTime = gunModule.f_effectTime;
            f_tickDamage = gunModule.f_tickDamage;
            f_effectTime = gunModule.f_effectTime;
            i_amountOfTicks = gunModule.i_amountOfTicks;
            f_slowPercent = gunModule.f_slowPercent;
            i_chainCount = gunModule.i_chainCount;
            f_chainLength = gunModule.f_chainLength;
            f_vampirePercent = gunModule.f_vampirePercent;

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
    }
}
