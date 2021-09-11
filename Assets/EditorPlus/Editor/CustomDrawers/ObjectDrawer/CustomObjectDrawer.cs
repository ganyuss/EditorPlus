using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {

    public interface IFrameworkEditor {
        public void OnEnable(IEnumerable<Object> targets);
        public void OnInspectorGUIBefore(IEnumerable<Object> targets);
        public void OnInspectorGUIAfter(IEnumerable<Object> targets);
    }
    
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class CustomObjectDrawer : UnityEditor.Editor
    {
        private List<IFrameworkEditor> Editors = new List<IFrameworkEditor> {
            new ButtonFrameworkEditor()
        };

        private void OnEnable() {
            foreach (var editor in Editors) {
                editor.OnEnable(targets);
            }
        }
        
        public override void OnInspectorGUI() {
            foreach (var editor in Editors) {
                editor.OnInspectorGUIBefore(targets);
            }

            DrawDefaultInspector();
            
            foreach (var editor in Editors) {
                editor.OnInspectorGUIAfter(targets);
            }
        }
    }
}