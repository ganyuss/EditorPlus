using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class HideInPlayModeAttribute : PropertyAttribute
    { }
    
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class DisableInPlayModeAttribute : PropertyAttribute
    { }
}

