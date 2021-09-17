using System;
using JetBrains.Annotations;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class OnEditorGUIAttribute : PropertyAttribute {

    }
}
