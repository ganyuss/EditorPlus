using EditorPlus;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class DisabledDecorator : DecoratorBase<DisabledAttribute> {

        public override OrderValue Order => OrderValue.VeryFirst;
        
        private bool guiEnabled;

        public override Rect OnBeforeGUI(Rect position, SerializedProperty property, GUIContent label) {
            guiEnabled = GUI.enabled;
            GUI.enabled = false;
            return position;
        }

        public override Rect OnAfterGUI(Rect position, SerializedProperty property, GUIContent label) {
            GUI.enabled = guiEnabled;
            return position;
        }
    }
}
