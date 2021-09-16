using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfAttribute : PropertyAttribute {
        public string MemberName;
        public object OptionalTargetValue;

        public DisableIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public DisableIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfAttribute : PropertyAttribute {
        public string MemberName;
        public object OptionalTargetValue;

        public EnableIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public EnableIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
}

