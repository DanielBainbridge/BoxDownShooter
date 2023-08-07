using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Gun
{
    [CreateAssetMenu(fileName = "GunModule", menuName = "Gun Module")]
    public class GunModule : ScriptableObject
    {
        /// <summary>
        /// Enum setups for further use in variables
        /// </summary>
        #region EnumAndStructSetup
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
            Multishot,
            Buckshot,
            Spray,
            Wave,
            Count
        }

        //Clip Module Enum
        public enum BulletEffect
        {
            None,
            DamageOverTime, //fire
            Slow, // ice
            Chain, //chain ricochets electricty
            Vampire, //health steal
            Count
        }

        [Serializable]
        public struct BulletTraitInfo
        {
            //bullet trait deppendent
            [Rename("Bullet Type")] public BulletTrait e_bulletTrait;
            [Rename("Pierce Count")] public int i_pierceCount;
            [Rename("Explosion Diameter")] public float f_explosionSize;
            [Rename("Explosion Prefab")] public GameObject C_explosionPrefab;
            [Rename("Explosion Knockback Distance")] public float f_explosionKnockbackDistance;
            [Rename("Explosion Linger Time")] public float f_explosionLingerTime;
            [Rename("Homing Strength"), Range(0, 1)] public float f_homingStrength;
            [Rename("Homing Delay")] public float f_homingDelayTime;
        }

        [Serializable]
        public struct BulletEffectInfo
        {
            //bullet effect dependent
            [Rename("Bullet Effect")] public BulletEffect e_bulletEffect;
            [Rename("Effect Time")] public float f_effectTime;
            [Rename("Damage Over Time - Damage Per Tick")] public float f_tickDamage;
            [Rename("Damage Over Time - Tick Count")] public int i_amountOfTicks;
            [Rename("Slow Percentage"), Range(0, 1)] public float f_slowPercent;
            [Rename("Enemy Chain Count")] public int i_chainCount;
            [Rename("Max Chain Length")] public float f_chainLength;
            [Rename("Ricochet Count")] public int i_ricochetCount;
            [Rename("Health Steal Percentage"), Range(0, 1)] public float f_vampirePercent;
        }

        [Serializable]
        public struct ShotPatternInfo
        {
            //shot pattern dependent
            [Rename("Shot Pattern")] public ShotPattern e_shotPattern;
            [Rename("Random Spread")] public bool b_randomSpread;
            [Rename("Shot Count")] public int i_shotCount;
            [Rename("Multi Shot Distance")] public float f_multiShotDistance;
            [Rename("Spread Angle")] public float f_maxAngle;
            [Rename("Wave Speed")] public float f_waveSpeed;
        }
        #endregion

        #region Variables
        //Global
        [Rename("Module Prefab")] public GameObject C_meshPrefab;
        [Rename("Module Type")] public ModuleSection e_moduleType;

        //public Gun.FireType e_fireType;

        //Trigger Group
        [Rename("Base Damage")] public float f_baseDamage;
        [Rename("Bullets Fired Per Second")] public float f_fireRate;
        [Rename("Bullet Speed")] public float f_bulletSpeed;
        [Rename("Knock Back")] public float f_knockBack;
        public BulletEffectInfo S_bulletEffectInformation;


        //Clip Group
        [Rename("Reload Time")] public float f_reloadSpeed;
        [Rename("Movement Penalty")] public float f_movementPenalty;
        [Rename("Clip Size")] public int i_clipSize;
        public BulletTraitInfo S_bulletTraitInformation;


        //Barrel Group
        [Rename("Bullet Size")] public float f_bulletSize;
        [Rename("Bullet Range")] public float f_bulletRange;
        [Rename("Recoil Distance")] public float f_recoil;
        [Rename("Burst Shot")] public bool b_burstTrue;
        [Rename("Burst Count")] public int i_burstCount;
        [Rename("Burst Interval")] public float f_burstInterval;
        public ShotPatternInfo S_shotPatternInformation;
        #endregion

        public void Spawn(Vector3 worldPos)
        {
            GameObject newGunModule = Instantiate(C_meshPrefab, worldPos, Quaternion.identity);
            newGunModule.name = name;
            newGunModule.tag = "Gun Module";
            newGunModule.layer = 6;
        }

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
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("b_burstTrue"));
                    if (serializedObject.FindProperty("b_burstTrue").boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("i_burstCount"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("f_burstInterval"));
                    }
                    EditorGUILayout.Space(10);

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("e_bulletEffect"));

                    Assert.IsFalse(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("e_bulletEffect").enumValueIndex == (int)GunModule.BulletEffect.Count, "A Bullet Effect cannot have the type 'Count' this is for programming use");



                    switch (serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("e_bulletEffect").enumValueIndex)
                    {
                        case 0:
                            break;
                        //do variables for Damage Over Time
                        case 1:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("f_effectTime"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("f_tickDamage"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("i_amountOfTicks"));
                            break;
                        //do variables for Slow
                        case 2:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("f_effectTime"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("f_slowPercent"));
                            break;

                        //do variables for Lightning
                        case 3:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("i_chainCount"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("f_chainLength"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("i_ricochetCount"));
                            break;
                        //do variables for Vampire
                        case 4:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("f_effectTime"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletEffectInformation").FindPropertyRelative("f_vampirePercent"));
                            break;
                    }
                    break;
                case 1:
                    //do certain variables yada yada
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_reloadSpeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_movementPenalty"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("i_clipSize"));
                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("e_bulletTrait"));

                    Assert.IsFalse(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("e_bulletTrait").enumValueIndex == (int)GunModule.BulletTrait.Count, "A Bullet Trait cannot have the type 'Count' this is for programming use");

                    switch (serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("e_bulletTrait").enumValueIndex)
                    {
                        //do variables for Standard
                        case 0:

                            break;
                        //do variables for Pierce
                        case 1:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("i_pierceCount"));

                            break;
                        //do variables for Explosive
                        case 2:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("C_explosionPrefab"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("f_explosionSize"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("f_explosionKnockbackDistance"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("f_explosionLingerTime"));

                            break;
                        //do variables for Homing
                        case 3:

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("f_homingStrength"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_bulletTraitInformation").FindPropertyRelative("f_homingDelayTime"));
                            break;
                    }
                    break;
                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_bulletSize"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_bulletRange"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("f_recoil"));


                    EditorGUILayout.Space(10);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("e_shotPattern"));
                    serializedObject.ApplyModifiedProperties();
                    Assert.IsFalse(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("e_shotPattern").enumValueIndex == (int)GunModule.ShotPattern.Count, "A Shot Pattern cannot have the type 'Count' this is for programming use");
                    switch (serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("e_shotPattern").enumValueIndex)
                    {
                        case 0:
                            //do variables for Straight


                            break;
                        case 1:
                            //do variables for Multi
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("i_shotCount"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("f_multiShotDistance"));

                            break;
                        case 2:
                            //do variables for Buckshot

                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("i_shotCount"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("f_maxAngle"));
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("b_randomSpread"));
                            break;
                        case 3:
                            //do variables for Spray                            
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("f_maxAngle"));

                            break;

                        case 4:
                            //do variables for Wave
                            EditorGUILayout.PropertyField(serializedObject.FindProperty("S_shotPatternInformation").FindPropertyRelative("f_maxAngle"));


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