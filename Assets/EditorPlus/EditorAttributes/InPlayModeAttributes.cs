using System;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(AttributeTargets.Field)]
    public class HideInPlayModeAttribute : PropertyAttribute
    { }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class DisableInPlayModeAttribute : PropertyAttribute
    { }
}

