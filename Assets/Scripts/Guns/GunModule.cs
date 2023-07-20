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

        //Trigger Module Enum
        public enum BulletTrait
        {
            Standard,
            Pierce,
            Explosive,
            Homing,
            Count
        }

        //Barrel Module Enum
        public enum ShotPattern
        {
            Straight,
            Burst,
            Spread,
            Spray,
            Count
        }

        //Clip Module Enum
        public enum BulletEffect
        {
            DamageOverTime,
            Slow,
            Freeze,
            Chain, //chain ricochets
            Vampire,
            Count
        }


        //Global
        [Rename("Module Prefab")] public GameObject C_meshPrefab;
        [Rename("Module Type")] public ModuleSection e_moduleType;

        //public Gun.FireType e_fireType;

        //Trigger Group
        [Rename("Base Damage")] public float f_baseDamage;
        [Rename("Fire Rate")] public float f_fireRate;
        [Rename("Bullet Speed")] public float f_bulletSpeed;
        [Rename("Knock Back")] public float f_knockBack;
        [Rename("Bullet Type")] public BulletTrait e_bulletTrait;
        //bullet trait deppendent
        [Rename("Pierce Count")] public int i_pierceCount;
        [Rename("Explosion Diameter")] public float f_explosionSize;
        [Rename("Explosion Prefab")] public GameObject C_explosionPrefab;
        [Rename("Explosion Knockback Distance")] public float f_explosionKnockbackDistance;
        [Rename("Explosion Linger Time")] public float f_explosionLingerTime;
        [Rename("Homing Strength"), Range(0, 1)] public float f_homingStrength;
        [Rename("Homing Delay")] public float f_homingDelayTime;

        //Clip Group
        [Rename("Reload Speed")] public float f_reloadSpeed;
        [Rename("Movement Penalty")] public float f_movementPenalty;
        [Rename("Clip Size")] public int i_clipSize;
        [Rename("Bullet Effect")] public BulletEffect e_bulletEffects;
        //bullet effect dependent
        [Rename("Effect Time")] public float f_effectTime;
        [Rename("Damage Over Time - Damage Per Tick")] public float f_tickDamage;
        [Rename("Damage Over Time - Tick Count")] public int i_amountOfTicks;
        [Rename("Slow Percentage"), Range(0, 1)] public float f_slowPercent;
        [Rename("Enemy Chain Count")] public int i_chainCount;
        [Rename("Max Chain Length")] public float f_chainLength;
        [Rename("Health Steal Percentage"), Range(0, 1)] public float f_vampirePercent;


        //Barrel Group
        [Rename("Shot Pattern")] public ShotPattern e_shotPattern;
        [Rename("Bullet Size")] public float f_bulletSize;
        [Rename("Bullet Range")] public float f_bulletRange;
        [Rename("Recoil Distance")] public float f_recoil;
        //shot pattern dependent
        [Rename("Burst Count")] public int i_burstCount;
        [Rename("Burst Interval")] public float f_burstInterval;
        [Rename("Random Spread")] public bool b_randomSpread;
        [Rename("Spread Angle")] public float f_spreadMaxAngle;
        [Rename("Spread Count")] public int i_spreadCount;

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
    using UnityEngine.Assertions;

    [CustomEditor(typeof(GunModule))]
    class BoxDownGunEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("C_meshPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("e_moduleType"));

            Assert.IsFalse(serializedObject.FindProperty("e_moduleType").enumValueIndex == (int)GunModule.ModuleSection.Count, "A Gun Module cannot have the type 'Count' this is for programming use");
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Variables:", EditorStyles.boldLabel);

            switch (serializedObject.FindProperty("e_moduleType").enumValueIndex)
            {
                case 0:
                    //do certain variables yada yada
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_baseDamage"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_fireRate"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_bulletSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_knockBack"));
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("e_bulletTrait"));
                    Assert.IsFalse(serializedObject.FindProperty("e_bulletTrait").enumValueIndex == (int)GunModule.BulletTrait.Count, "A Bullet Trait cannot have the type 'Count' this is for programming use");


                    switch (serializedObject.FindProperty("e_bulletTrait").enumValueIndex)
                    {
                        //do variables for Standard
                        case 0:

                            break;
                        //do variables for Pierce
                        case 1:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("i_pierceCount"));

                            break;
                        //do variables for Explosive
                        case 2:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("C_explosionPrefab"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_explosionSize"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_explosionKnockbackDistance"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_explosionLingerTime"));

                            break;
                        //do variables for Homing
                        case 3:

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_homingStrength"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_homingDelayTime"));
                            break;
                    }
                    break;
                case 1:
                    //do certain variables yada yada
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_reloadSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_movementPenalty"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("i_clipSize"));
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("e_bulletEffects"));
                    Assert.IsFalse(serializedObject.FindProperty("e_bulletEffects").enumValueIndex == (int)GunModule.BulletEffect.Count, "A Bullet Effect cannot have the type 'Count' this is for programming use");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_effectTime"));

                    switch (serializedObject.FindProperty("e_bulletEffects").enumValueIndex)
                    {
                        //do variables for Damage Over Time
                        case 0:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_tickDamage"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("i_amountOfTicks"));
                            break;
                        //do variables for Slow
                        case 1:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_slowPercent"));
                            break;
                        //do variables for Freeze
                        case 2:
                            break;

                        //do variables for Chain
                        case 3:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("i_chainCount"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_chainLength"));

                            break;
                        //do variables for Vampire
                        case 4:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_vampirePercent"));
                            break;
                    }

                    break;


                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_bulletSize"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_bulletRange"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_recoil"));
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("e_shotPattern"));
                    serializedObject.ApplyModifiedProperties();
                    Assert.IsFalse(serializedObject.FindProperty("e_shotPattern").enumValueIndex == (int)GunModule.ShotPattern.Count, "A Shot Pattern cannot have the type 'Count' this is for programming use");
                    switch (serializedObject.FindProperty("e_shotPattern").enumValueIndex)
                    {
                        case 0:
                            //do variables for Straight

                            break;
                        case 1:
                            //do variables for Burst
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("i_burstCount"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_burstInterval"));

                            break;
                        case 2:
                            //do variables for Spread

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("i_spreadCount"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("f_spreadMaxAngle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("b_randomSpread"));
                            break;
                        case 3:
                            //do variables for Spray                            

                            break;
                    }


                    break;
            }

            // we need to draw the modules assigned varaibles based on enum, then draw certain values after

            serializedObject.ApplyModifiedProperties();

        }
    }
#endif
}