using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public class HideIfDecorator : DecoratorBase<HideIfAttribute> {
        
        public override OrderValue Order => OrderValue.VeryFirst;
        public override bool ShowProperty(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, CurrentAttribute.MemberName, out var targetObject, out var targetMember);
            return !EditorUtils.GetGeneralValue<bool>(targetObject, targetMember);
        }
    }
}