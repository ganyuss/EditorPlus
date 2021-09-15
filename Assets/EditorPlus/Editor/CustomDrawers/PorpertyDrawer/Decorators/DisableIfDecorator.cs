using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace EditorPlus.Editor {
    public class DisableIfDecorator : DisableIfDecoratorBase<DisableIfAttribute> {
        
        protected override bool Disable(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, CurrentAttribute.MemberName, out var targetObject, out var targetMember);
            return EditorUtils.GetGeneralValue<bool>(targetObject, targetMember);
        }
    }
}