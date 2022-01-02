using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// This class is the main property drawer for the plugin. If the property is
    /// of <see cref="SerializedPropertyType.Generic">generic</see> type, it draws
    /// everything using the <see cref="SerializedPropertyDrawer"/>. Otherwise it draws
    /// the field normally using the <see cref="EditorGUI.PropertyField(Rect, SerializedProperty, GUIContent)"/> method.
    /// <br /><br />
    /// This allows for generic fields to be displayed with class decorators,
    /// and to be drawn with the better Editor+ lists. 
    /// </summary>
#if !EDITOR_PLUS_DISABLE_EDITOR
    [CustomPropertyDrawer(typeof(object), true)]
#endif
    public partial class EditorPlusObjectPropertyDrawer : PropertyDrawer {
            
        private readonly SerializedPropertyDrawer Drawer = new SerializedPropertyDrawer();
            
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.Generic) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            Drawer.Draw(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.Generic) {
                return EditorGUI.GetPropertyHeight(property, label);
            }
                
            return Drawer.GetPropertyHeight(property, label);
        }
    }
}
