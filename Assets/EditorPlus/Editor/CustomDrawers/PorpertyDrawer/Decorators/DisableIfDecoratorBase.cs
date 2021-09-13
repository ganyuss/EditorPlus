using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public abstract class DisableIfDecoratorBase<Attr> : DecoratorBase<Attr> where Attr : PropertyAttribute {
        public override OrderValue Order => OrderValue.VeryFirst;

        protected abstract bool Disable([CanBeNull] SerializedProperty property);
        
        private bool guiEnabled;

        public override Rect OnBeforeGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            if (Disable(property)) {
                guiEnabled = GUI.enabled;
                GUI.enabled = false;
            }
            
            return position;
        }

        public override Rect OnAfterGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (Disable(property))
                GUI.enabled = guiEnabled;
            
            return position;
        }
    }
}
