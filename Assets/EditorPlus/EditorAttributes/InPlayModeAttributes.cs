using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(DecoratorAttribute.Targets)]
    public class HideInPlayModeAttribute : PropertyAttribute
    { }
    
    [AttributeUsage(DecoratorAttribute.Targets)]
    public class DisableInPlayModeAttribute : PropertyAttribute
    { }
}

