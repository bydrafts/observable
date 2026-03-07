#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace LichGame.Editor
{
    [CustomPropertyDrawer(typeof(Drafts.Observable<>), true)]
    public class ObservableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var value = property.FindPropertyRelative("value");
            return EditorGUI.GetPropertyHeight(value, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var value = property.FindPropertyRelative("value");
            EditorGUI.BeginProperty(position, label, value);
            EditorGUI.PropertyField(position, value, label);
            EditorGUI.EndProperty();
        }
    }
}
#endif