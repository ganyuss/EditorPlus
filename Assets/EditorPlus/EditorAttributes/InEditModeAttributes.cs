using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(DecoratorAttribute.Targets)]
    public class HideInEditModeAttribute : PropertyAttribute
    { }
    
    [AttributeUsage(DecoratorAttribute.Targets)]
    public class DisableInEditModeAttribute : PropertyAttribute
    { }
}

