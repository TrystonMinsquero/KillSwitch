using UnityEngine;
using UnityEditor;
using System;

//This was obtained from https://gist.github.com/aarthificial/f2dbb58e4dbafd0a93713a380b9612af
//Thank you https://gist.github.com/aarthificial

[Serializable]
public struct Optional<T>
{
    [SerializeField] private bool enabled;
    [SerializeField] private T value;

    public Optional(T initValue)
    {
        enabled = true;
        value = initValue;
    }

    public bool Enabled => enabled;
    public T Value => value;
}

namespace Editor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");
            return EditorGUI.GetPropertyHeight(valueProperty);
        }

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label
        )
        {
            var valueProperty = property.FindPropertyRelative("value");
            var enabledProperty = property.FindPropertyRelative("enabled");

            EditorGUI.BeginProperty(position, label, property);
            position.width -= 24;
            EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            EditorGUI.EndDisabledGroup();

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            position.x -= position.width;
            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
