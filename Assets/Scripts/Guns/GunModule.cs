using System.Collections;
using UnityEngine;
using Utility;

namespace Gun
{
    //TO DO CHANGE WIP WHEN READY
    [CreateAssetMenu(fileName = "GunModule", menuName = "Gun Module: WIP")]
    public class GunModule : ScriptableObject
    {
        /// <summary>
        /// Enum setups for further use in variables
        /// </summary>

        public enum ModuleSection
        {
            Trigger,
            Clip,
            Barrel,
            Count
        }

        public enum ShotPattern
        {
            Straight,
            Burst,
            Spread,
            Spray,
            Count
        }

        public enum BulletEffect
        {
            DamageOverTime,
            Slow,
            Paralysis,
            Count
        }


        //Global
        [Rename("Module Mesh Prefab")] GameObject C_meshPrefab;
        public ModuleSection e_moduleType;

        public Gun.FireType e_fireType;

        //Trigger Group
        [Rename("Base Damage")] public float f_baseDamage;
        [Rename("Fire Rate")] public float f_fireRate;
        [Rename("Bullet Speed")] public float f_bulletSpeed;
        [Rename("Knock Back")] public float f_knockBack;

        //Clip Group
        [Rename("Reload Speed")] public float f_reloadSpeed;
        [Rename("Movement Penalty")] public float f_movementPenalty;
        [Rename("Clip Size")] public int i_clipSize;
        [Rename("Bullet Effect")] BulletEffect e_bulletEffects;

        //Barrel Group
        [Rename("Burst Count")] public int i_burstCount;
        [Rename("Burst Interval")] public float f_burstInterval;
        [Rename("Random Spread")] public bool b_randomSpread;
        [Rename("Spread Angle")] public float f_spreadAngle;
        [Rename("Spread Count")] public int i_spreadCount;
        [Rename("Recoil Distance")] public float f_recoil;
        [Rename("Bullet Range")] public float f_bulletRange;
        [Rename("Bullet Size")] public float f_bulletSize;



        //public AnimationCurve C_bulletArc;
        //public float f_bulletArcFrequency;
        //public float f_bulletArcAmplitude;



    }
}
namespace Guns.CustomEditor
{
    using Gun;
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(GunModule))]
    class BoxDownGunEditor : Editor
    {
        // This needs to be fancier
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // we need to draw the modules assigned varaibles based on enum, then draw certain values after
        }
    }
#endif
}