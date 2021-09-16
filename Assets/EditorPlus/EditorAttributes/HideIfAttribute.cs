using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute {
        public string MemberName;
        public object OptionalTargetValue;

        public HideIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public HideIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute {
        public string MemberName;
        public object OptionalTargetValue;

        public ShowIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public ShowIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
}
