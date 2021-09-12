using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(
        AttributeTargets.Method 
        | AttributeTargets.Field
        | AttributeTargets.Property)]
    public class CustomSpaceAttribute : PropertyAttribute {
        public float SpaceBefore = 18f;
        public float SpaceAfter = 0f;

        public CustomSpaceAttribute() { }

        public CustomSpaceAttribute(float spaceBefore) {
            SpaceBefore = spaceBefore;
        }
        
        public CustomSpaceAttribute(float spaceBefore, float spaceAfter) : this(spaceBefore) {
            SpaceAfter = spaceAfter;
        }
    }
}
