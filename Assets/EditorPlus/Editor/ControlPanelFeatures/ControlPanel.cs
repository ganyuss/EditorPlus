using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorPlus.Editor {
    public class ControlPanel : ScriptableObject {
        public List<string> PropertyAttributeNamespaceBlackList = new List<string>();
    }

    // We use a custom editor here instead of button attributes, so that the 
    // editor will work no matter what
    [CustomEditor(typeof(ControlPanel))]
    public class ControlPanelEditor : UnityEditor.Editor {
        private ReorderableList AttributeBlacklist;

        private ControlPanel Panel => (ControlPanel) target;

        private void OnEnable() {
            AttributeBlacklist = new ReorderableList(Panel.PropertyAttributeNamespaceBlackList, typeof(string));
        }

        public override void OnInspectorGUI() {
            
            AttributeBlacklist.DoLayoutList();
            DrawButton("Update the PropertyAttribute list",
                () => { ControlPanelFeatures.GenerateCustomDrawerAttributes(Panel.PropertyAttributeNamespaceBlackList); });
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