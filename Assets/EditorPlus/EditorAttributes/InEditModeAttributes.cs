using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class HideInEditModeAttribute : PropertyAttribute
    { }
    
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class DisableInEditModeAttribute : PropertyAttribute
    { }
}

