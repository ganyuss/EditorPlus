using System;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class ControlPanel : ScriptableObject
    { }

    // We use a custom editor here instead of button attributes, so that the 
    // editor will work no matter what
    [CustomEditor(typeof(ControlPanel))]
    public class ControlPanelEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            DrawButton("Update the CustomPropertyAttribute list", ControlPanelFeatures.GenerateCustomDrawerAttributes);
        }

        private void DrawButton(string buttonName, Action callback) {
            if (GUILayout.Button(buttonName)) {
                callback.Invoke();
            }
        }

        protected override bool ShouldHideOpenButton() {
            return true;
        }

        public override bool HasPreviewGUI() {
            return false;
        }
    }
}