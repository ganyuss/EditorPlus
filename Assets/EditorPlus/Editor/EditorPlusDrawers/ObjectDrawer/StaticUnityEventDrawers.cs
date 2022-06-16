using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor
{
    [CustomEditor(typeof(StaticUnityEventComponent))]
    public class StaticUnityEventComponentDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var idProperty = serializedObject.FindProperty("EventId");
            var eventProperty = serializedObject.FindProperty("OnEventInvoked");

            Rect idSelectorRect = EditorGUILayout.GetControlRect(true, StaticUnityEventIdSelectorDrawer.GetHeight());
            StaticUnityEventIdSelectorDrawer.DrawIdSelector(idSelectorRect, idProperty);

            EditorGUILayout.PropertyField(eventProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }

    public class StaticUnityEventInvokerDrawer : PropertyDrawerBase<StaticUnityEventInvoker>
    {
        public override float GetHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUIUtility.singleLineHeight + StaticUnityEventIdSelectorDrawer.GetHeight();

            return EditorGUIUtility.singleLineHeight;
        }

        public override Rect OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect foldoutRect = new Rect(position) { height = EditorGUIUtility.singleLineHeight };
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
            
            position.ToBottomOf(foldoutRect);

            EditorGUI.indentLevel += 1;
            if (property.isExpanded)
            {
                SerializedProperty idProperty = property.FindPropertyRelative("EventId");
                Rect selectorRect = new Rect(position) { height = StaticUnityEventIdSelectorDrawer.GetHeight() };
                StaticUnityEventIdSelectorDrawer.DrawIdSelector(selectorRect, idProperty);

                position.ToBottomOf(selectorRect);
            }
            EditorGUI.indentLevel -= 1;
            
            return position;
        }
    }

    internal static class StaticUnityEventIdSelectorDrawer
    {
        public static float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public static void DrawIdSelector(Rect position, SerializedProperty idProperty)
        {
            List<string> availableIds = StaticEventValueSettings.instance.EventIds;
            if (availableIds.Count == 0)
                availableIds = new List<string> { "" };
            
            int idIndex = availableIds.IndexOf(idProperty.stringValue);

            if (idIndex < 0 || idIndex >= availableIds.Count)
                idIndex = 0;

            int newIdIndex = EditorGUI.Popup(position, "Select ID", idIndex, availableIds.Concat(new[] { "Add a new ID..." }).ToArray());

            if (newIdIndex == availableIds.Count)
            {
                SettingsService.OpenProjectSettings(EditorPlusSettingsProvider.SettingsPath);
            }
            else
            {
                idProperty.stringValue = availableIds[newIdIndex];
            }
        }
    }
}
