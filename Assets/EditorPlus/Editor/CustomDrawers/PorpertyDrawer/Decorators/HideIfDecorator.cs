using System;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public abstract class HideIfDecoratorBase<Attr> : DecoratorBase<Attr> where Attr : PropertyAttribute {
        
        public override OrderValue Order => OrderValue.Regular;
        private bool AttributeValueCorrect = true;
        
        protected abstract string MemberName { get; }
        protected abstract bool PropertyShown(SerializedProperty property);
        
        public sealed override bool ShowProperty(SerializedProperty property) {
            try {
                return PropertyShown(property);
            }
            catch (Exception) {
                AttributeValueCorrect = false;
                return true;
            }
        }

        private string ErrorText => $"HideIf: Member \"{MemberName}\" not found, or value not correct. " +
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

    public class HideIfDecorator : HideIfDecoratorBase<HideIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        protected override bool PropertyShown(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, CurrentAttribute.MemberName, out var targetObject, out var targetMember);
            return !Equals(EditorUtils.GetGeneralValue<object>(targetObject, targetMember), CurrentAttribute.OptionalTargetValue);
        }
    }
    
    public class ShowIfDecorator : HideIfDecoratorBase<ShowIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        protected override bool PropertyShown(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, CurrentAttribute.MemberName, out var targetObject, out var targetMember);
            return Equals(EditorUtils.GetGeneralValue<object>(targetObject, targetMember), CurrentAttribute.OptionalTargetValue);
        }
    }
}