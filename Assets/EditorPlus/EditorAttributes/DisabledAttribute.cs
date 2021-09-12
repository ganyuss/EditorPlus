
using System;
using UnityEngine;

namespace EditorPlus {
    
    
    [AttributeUsage(
        AttributeTargets.Method 
        | AttributeTargets.Field
        | AttributeTargets.Property)]
    public class DisabledAttribute : PropertyAttribute { }
}
