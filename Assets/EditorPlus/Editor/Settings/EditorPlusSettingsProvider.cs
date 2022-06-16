using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor
{
    public static class EditorPlusSettingsProvider
    {
        public const string SettingsPath = "Project/EditorPlus";


        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            var staticEventIdSettings = new SerializedObject(StaticEventValueSettings.instance);
            ListPropertyDrawer drawer = new ListPropertyDrawer
            {
                AlwaysExpanded = true
            };

            var provider = new SettingsProvider(SettingsPath, SettingsScope.Project)
            {
                label = "EditorPlus Settings",
                guiHandler = searchContext =>
                {
                    GUI.enabled = true;
                    var ListProperty = staticEventIdSettings.FindProperty(nameof(StaticEventValueSettings._eventIds));
                    var listPropertyLabel = new GUIContent("Available static event IDs");
                    var listHeight = drawer.GetHeight(ListProperty, listPropertyLabel);

                    drawer.OnGUI(EditorGUILayout.GetControlRect(false, listHeight), ListProperty, listPropertyLabel);

                    staticEventIdSettings.ApplyModifiedProperties();
                },

                keywords = new HashSet<string>(new[] { "Editor", "Drawer", "Static", "UnityEvent" })
            };

            return provider;
        }
    }
}
