using UnityEngine;

namespace Utility
{
    //Setup for attribute "Rename"
    public class RenameAttribute : PropertyAttribute
    {
        public string s_newName { get; private set; }
        public RenameAttribute(string name)
        {
            s_newName = name;
        }
    }
}
namespace Utility.CustomEditor
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    //Inspector instructions for a [Rename()] variable
    [CustomPropertyDrawer(typeof(RenameAttribute))]
    public class RenameAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, new GUIContent((attribute as RenameAttribute).s_newName));
        }
    }
}