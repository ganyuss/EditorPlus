using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public abstract class DisableIfDecoratorBase<Attr> : DecoratorBase<Attr> where Attr : PropertyAttribute {
        public override OrderValue Order => OrderValue.VeryFirst;

        protected abstract bool Disable([CanBeNull] SerializedProperty property);
        
        private bool guiEnabled;

        protected override Rect OnBeforeGUI(Rect position, string memberPath, SerializedProperty property) {
            
            if (Disable(property)) {
                guiEnabled = GUI.enabled;
                GUI.enabled = false;
            }
            
            return position;
        }

        protected override Rect OnAfterGUI(Rect position, string memberPath, SerializedProperty property) {
            if (Disable(property))
                GUI.enabled = guiEnabled;
            
            return position;
        }
    }
}
