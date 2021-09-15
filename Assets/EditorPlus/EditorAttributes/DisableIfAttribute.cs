using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfAttribute : PropertyAttribute {
        public string MemberName;

        public DisableIfAttribute(string memberName) {
            MemberName = memberName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class EnableIfAttribute : PropertyAttribute {
        public string MemberName;

        public EnableIfAttribute(string memberName) {
            MemberName = memberName;
        }
    }
}

