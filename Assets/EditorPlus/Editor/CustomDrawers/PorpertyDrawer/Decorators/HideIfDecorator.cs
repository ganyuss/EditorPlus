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

        protected override Rect OnBeforeGUI(Rect position, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.OnBeforeGUI(position, memberPath, property) : EditorUtils.HelpBox.Draw(position, ErrorText, HelpBoxType.Error);
        }
        
        protected override Rect OnAfterGUI(Rect position, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.OnAfterGUI(position, memberPath, property) : position;
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