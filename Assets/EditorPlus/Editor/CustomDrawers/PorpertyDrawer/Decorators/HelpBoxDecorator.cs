using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class HelpBoxDecorator : DecoratorBase<HelpBoxAttribute> {
        public override float GetHeight(SerializedProperty property, GUIContent label) {
            return EditorUtils.HelpBox.GetHeight(CurrentAttribute.Text, CurrentAttribute.Type);
        }

        public override Rect OnBeforeGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (CurrentAttribute.Position == HelpBoxPosition.Before) {
                return DrawHelpBox(position);
            }

            return position;
        }

        public override Rect OnAfterGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (CurrentAttribute.Position == HelpBoxPosition.After) {
                return DrawHelpBox(position);
            }

            return position;
        }

        private Rect DrawHelpBox(Rect position) {
            return EditorUtils.HelpBox.Draw(position, CurrentAttribute.Text, CurrentAttribute.Type);
        }
    }
}