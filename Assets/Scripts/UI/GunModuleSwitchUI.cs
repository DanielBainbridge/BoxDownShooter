using Gun;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class GunModuleSwitchUI : MonoBehaviour
{

    //private string f_gunModuleName;
    //private Color S_moduleTypeColour;


    //private string s_statOneName;
    //private TMPro.TextMeshPro s_statOneText;
    //private float f_statOne;
    //private float f_statOneMax;
    //private Slider f_statOneSlider;

    //private string s_statTwoName;
    //private TMPro.TextMeshPro s_statTwoText;
    //private float f_statTwo;
    //private float f_statTwoMax;
    //private Slider f_statTwoSlider;

    //private string s_statThreeName;
    //private TMPro.TextMeshPro s_statThreeText;
    //private float f_statThree;
    //private float f_statThreeMax;
    //private Slider f_statThreeSlider;

    //private string s_stringStatName;
    //private string s_stringStat;
    //private TMPro.TextMeshPro C_stringText;

    //private RectTransform C_rectTransform;


    //[SerializeField, Rename("Stat Gradient")] private Gradient C_statGradient;

    //private void Start()
    //{
    //    C_rectTransform = GetComponent<RectTransform>();
    //}

    //public void TurnOnModuleSwitchUI(GunModule gunModule)
    //{
    //    gameObject.SetActive(true);
    //    UpdateStats(gunModule);
    //}
    //public void SetPosition(Vector3 position)
    //{
    //    if (C_rectTransform.rect.size.y + position.y > Screen.height)
    //    {

    //    }
    //    C_rectTransform.position = position;
    //}

    //private void UpdateStats(GunModule gunModule)
    //{
    //    switch (gunModule.e_moduleType)
    //    {
    //        case GunModule.ModuleSection.Trigger:
    //            SetStatOne(gunModule.f_baseDamage, "Damage:", false);
    //            SetStatTwo(gunModule.f_fireRate, "Fire Rate:", false);
    //            SetStatThree(gunModule.f_bulletSpeed, "Bullet Speed:", false);
    //            SetStringStat(gunModule.S_bulletEffectInformation.e_bulletEffect.DisplayName(), "Element:");
    //            break;
    //        case GunModule.ModuleSection.Clip:
    //            SetStatOne(gunModule.i_clipSize, "Clip Size:", false);
    //            SetStatTwo(gunModule.f_reloadSpeed, "Reload Time:", true);
    //            SetStatThree(gunModule.f_movementPenalty, "Movement Penalty:", true);
    //            SetStringStat(gunModule.S_bulletTraitInformation.e_bulletTrait.DisplayName(), "Bullet Type:");
    //            break;
    //        case GunModule.ModuleSection.Barrel:
    //            SetStatOne(gunModule.f_bulletSize, "Bullet Size:", false);
    //            SetStatTwo(gunModule.f_bulletRange, "Range:", false);
    //            SetStatThree(gunModule.f_recoil, "Recoil:", true);
    //            SetStringStat(gunModule.S_shotPatternInformation.e_shotPattern.DisplayName(), "Shot Pattern:");
    //            break;
    //    }
    //}

    //public void TurnOffModuleSwitchUI()
    //{
    //    gameObject.SetActive(false);
    //}

    //private void SetStatOne(float statOne, string statName, bool negativeStat)
    //{
    //    s_statOneName = statName;
    //    f_statOne = statOne;
    //    ColourPickGradient(negativeStat, 1);
    //}
    //private void SetStatTwo(float statTwo, string statName, bool negativeStat)
    //{
    //    s_statTwoName = statName;
    //    f_statTwo = statTwo;
    //    ColourPickGradient(negativeStat, 2);
    //}
    //private void SetStatThree(float statThree, string statName, bool negativeStat)
    //{
    //    s_statThreeName = statName;
    //    f_statThree = statThree;
    //    ColourPickGradient(negativeStat, 3);
    //}
    //private void SetStringStat(string stringStat, string statName)
    //{
    //    s_stringStatName = statName;
    //    s_stringStat = stringStat;
    //}

    //private void ColourPickGradient(bool negativeStat, int statNumber)
    //{
    //    float colourPick = 0;

    //    switch (statNumber)
    //    {
    //        case 1:
    //            colourPick = f_statOne / f_statOneMax;
    //            break;
    //        case 2:
    //            colourPick = f_statOne / f_statOneMax;
    //            break;
    //        case 3:
    //            colourPick = f_statOne / f_statOneMax;
    //            break;
    //        default:
    //            throw new ArgumentOutOfRangeException("Colour Picking Failed: Stat tried to update out of range");
    //    }
    //    if (negativeStat)
    //    {
    //        ExtraMaths.Map(0, 1, 1, 0, colourPick);
    //    }

    //    //evaluate is 0 - 1
    //    C_statGradient.Evaluate(colourPick);
    //    return;

    //}

}
