using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(AttributeTargets.Field)]
    public class HideInEditModeAttribute : PropertyAttribute
    { }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class DisableInEditModeAttribute : PropertyAttribute
    { }
}

