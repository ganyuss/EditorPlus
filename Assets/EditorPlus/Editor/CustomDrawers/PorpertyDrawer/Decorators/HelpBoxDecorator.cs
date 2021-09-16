using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class HelpBoxDecorator : DecoratorBase<HelpBoxAttribute> {
        public override float GetHeight(SerializedProperty property, GUIContent label) {
            return EditorUtils.HelpBox.GetHeight(CurrentAttribute.Text, CurrentAttribute.Type);
        }

        protected override Rect OnBeforeGUI(Rect position, string memberPath, SerializedProperty property) {
            if (CurrentAttribute.Position == HelpBoxPosition.Before) {
                return DrawHelpBox(position);
            }

            return position;
        }

        protected override Rect OnAfterGUI(Rect position, string memberPath, SerializedProperty property) {
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