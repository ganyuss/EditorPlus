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
}
