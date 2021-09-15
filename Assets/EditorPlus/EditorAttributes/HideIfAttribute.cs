using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute {
        public string MemberName;

        public HideIfAttribute(string memberName) {
            MemberName = memberName;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute {
        public string MemberName;

        public ShowIfAttribute(string memberName) {
            MemberName = memberName;
        }
    }
}
