using Gun;
using System;
using UnityEngine;
using Utility;

namespace Explosion
{
    public static class ExplosionGenerator
    {
        public static void MakeExplosion(Vector3 position, GameObject prefab,
            float size, float damage, float knockbackStrength, float lifeTime, GunModule gunModule = null)
        {
            GameObject newExplosion = Transform.Instantiate(prefab, position, Quaternion.Euler(0, ExtraMaths.FloatRandom(0, 360), 0));
            newExplosion.name = "Explosion";
            Explosion explosionRef = newExplosion.AddComponent<Explosion>();
            
            explosionRef.C_gunModuleCreator = gunModule;
            explosionRef.f_explosionSize = size;
            explosionRef.f_explosionDamage = damage;
            explosionRef.f_explosionKnockbackStrength = knockbackStrength;
            explosionRef.f_explosionLifeTime = lifeTime;
            explosionRef.InitialiseExplosion();            
        }
    }
}
