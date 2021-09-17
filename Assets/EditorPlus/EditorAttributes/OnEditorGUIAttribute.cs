using System;
using JetBrains.Annotations;
using UnityEngine;

namespace EditorPlus {
    [AttributeUsage(DecoratorAttribute.Targets)]
    [MeansImplicitUse]
    public class OnEditorGUIAttribute : PropertyAttribute {

    }
}
