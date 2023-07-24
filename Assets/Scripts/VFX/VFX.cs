using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Audio
{

    [CreateAssetMenu(fileName = "VFX", menuName = "VFXSystem/VFX")]
    public class VFX : ScriptableObject
    {
        [Header("Audio/Visual Prefab")]
        public GameObject m_prefab;
        [Header("Pitch Random Check")]
        public bool m_pitchChange;
        public float m_randomPitchMin = 0.9f;
        public float m_randomPitchMax = 1.1f;

        [Header("Parenting And Orientation Matching")]
        public bool m_attach;
        public bool m_orient;

        public bool m_soundPersistance;
        public GameObject Spawn(Transform t)
        {
            Transform parent = m_attach ? t : null;
            Quaternion orientation = m_orient ? t.rotation : Quaternion.identity;
            GameObject newFX = Instantiate(m_prefab, t.position, orientation, parent);
            if (newFX.GetComponent<AudioSource>() && m_pitchChange)
            {
                //changes pitch of sound component in a random range
                newFX.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            }
            if (!m_soundPersistance)
            {
                Despawn(newFX);
            }
            return newFX;
        }
        public GameObject Spawn(Transform t, float pitch, float volume)
        {
            Transform parent = m_attach ? t : null;
            Quaternion orientation = m_orient ? t.rotation : Quaternion.identity;
            GameObject newFX = Instantiate(m_prefab, t.position, orientation, parent);
            if (newFX.GetComponent<AudioSource>())
            {
                //changes pitch of sound component in a random range
                newFX.GetComponent<AudioSource>().pitch = pitch;
                newFX.GetComponent<AudioSource>().volume = volume;
            }
            if (!m_soundPersistance)
            {
                Despawn(newFX);
            }
            return newFX;
        }
        public void Despawn(GameObject effectToDestroy)
        {
            Destroy(effectToDestroy, 10);
        }
    }
}
namespace Audio.CustomInspector
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(VFX))]
    public class VFXEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif

}