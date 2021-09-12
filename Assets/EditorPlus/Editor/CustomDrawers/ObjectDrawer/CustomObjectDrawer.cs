using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {

    public interface IFrameworkEditor {
        public void OnEnable(List<Object> targets);
        public void OnInspectorGUIBefore(List<Object> targets);
        public void OnInspectorGUIAfter(List<Object> targets);
    }
    
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class CustomObjectDrawer : UnityEditor.Editor {
        private List<IFrameworkEditor> Editors;

        private void OnEnable() {
            Editors = TypeUtils.GetAllTypesImplementing(typeof(IFrameworkEditor))
                .Select(TypeUtils.CreateInstance<IFrameworkEditor>)
                .ToList();
            
            List<Object> targetList = targets.ToList();
            
            foreach (var editor in Editors) {
                editor.OnEnable(targetList);
            }
        }
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            List<Object> targetList = targets.ToList();
            
            foreach (var editor in Editors) {
                editor.OnInspectorGUIBefore(targetList);
            }

            DrawDefaultInspector();
            
            foreach (var editor in Editors) {
                editor.OnInspectorGUIAfter(targetList);
            }
        }
    }
}