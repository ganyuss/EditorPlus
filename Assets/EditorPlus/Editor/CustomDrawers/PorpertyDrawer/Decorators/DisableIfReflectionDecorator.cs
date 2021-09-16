using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace EditorPlus.Editor {
    public abstract class DisableIfReflectionDecoratorBase<Attr> : DisableIfDecoratorBase<Attr> where Attr : PropertyAttribute {
        
        private bool AttributeValueCorrect = true;
        
        protected abstract string MemberName { get; }
        protected abstract bool PropertyDisabled(SerializedProperty property);
        
        protected sealed override bool Disable(SerializedProperty property) {
            try {
                return PropertyDisabled(property);
            }
            catch (Exception) {
                AttributeValueCorrect = false;
                return true;
            }
        }

        private string ErrorText => $"DisableIf: Member \"{MemberName}\" not found, or value not correct. " +
                                    $"It should be the name of an instance bool field, property or method " +
                                    $"with no parameters.";
        
        public sealed override float GetHeight(SerializedProperty property, GUIContent label) {
            return AttributeValueCorrect ? base.GetHeight(property, label): EditorUtils.HelpBox.GetHeight(ErrorText, HelpBoxType.Error);
        }

        public sealed override Rect OnBeforeGUI(Rect position, SerializedProperty property, GUIContent label) {
            return AttributeValueCorrect ? base.OnBeforeGUI(position, property, label) : EditorUtils.HelpBox.Draw(position, ErrorText, HelpBoxType.Error);
        }
        
        public sealed override Rect OnAfterGUI(Rect position, SerializedProperty property, GUIContent label) {
            return AttributeValueCorrect ? base.OnAfterGUI(position, property, label) : position;
        }
    }

    public class DisableIfDecorator : DisableIfReflectionDecoratorBase<DisableIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        
        protected override bool PropertyDisabled(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, CurrentAttribute.MemberName, out var targetObject, out var targetMember);
            return EditorUtils.GetGeneralValue<bool>(targetObject, targetMember);
        }
    }
    
    public class EnableIfDecorator : DisableIfReflectionDecoratorBase<EnableIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        
        protected override bool PropertyDisabled(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, CurrentAttribute.MemberName, out var targetObject, out var targetMember);
            return !EditorUtils.GetGeneralValue<bool>(targetObject, targetMember);
        }
    }
}