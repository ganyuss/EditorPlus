using System;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public class HideIfDecorator : DecoratorBase<HideIfAttribute> {
        
        public override OrderValue Order => OrderValue.Regular;
        private bool AttributeValueCorrect = true;
        
        public override bool ShowProperty(SerializedProperty property) {
            try {
                EditorUtils.GetMemberInfo(property, CurrentAttribute.MemberName, out var targetObject, out var targetMember);
                return !EditorUtils.GetGeneralValue<bool>(targetObject, targetMember);
            }
            catch (Exception) {
                AttributeValueCorrect = false;
                return true;
            }
        }

        private string ErrorText => $"HideIf: Member \"{CurrentAttribute.MemberName}\" not found, or value not correct. " +
                                    $"It should be the name of an instance bool field, property or method " +
                                    $"with no parameters.";
        
        public override float GetHeight(SerializedProperty property, GUIContent label) {
            return AttributeValueCorrect ? base.GetHeight(property, label): EditorUtils.HelpBox.GetHeight(ErrorText, HelpBoxType.Error);
        }

        public override Rect OnBeforeGUI(Rect position, SerializedProperty property, GUIContent label) {
            return AttributeValueCorrect ? base.OnBeforeGUI(position, property, label) : EditorUtils.HelpBox.Draw(position, ErrorText, HelpBoxType.Error);
        }
        
        public override Rect OnAfterGUI(Rect position, SerializedProperty property, GUIContent label) {
            return AttributeValueCorrect ? base.OnAfterGUI(position, property, label) : position;
        }
    }
}