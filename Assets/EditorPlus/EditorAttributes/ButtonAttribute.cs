

using UnityEngine;

namespace EditorPlus
{
    using System;
    using JetBrains.Annotations;

    public enum ButtonSize
    {
        Small,
        Regular,
        Large,
        ExtraLarge
    }

    /// <summary>
    /// Attribute to create a button in the inspector for calling the method it is attached to.
    /// The method must have no arguments.
    /// </summary>
    /// <example><code>
    /// [Button]
    /// public void MyMethod()
    /// {
    ///     Debug.Log("Clicked!");
    /// }
    /// </code></example>
    [AttributeUsage(DecoratorAttribute.Targets)]
    [MeansImplicitUse]
    public sealed class ButtonAttribute : PropertyAttribute {
        public readonly string Name;

        public ButtonAttribute() { }

        public ButtonAttribute(string name) => Name = name;

        /// <summary>
        /// Indicates the size of the button.
        /// Defaults to <see cref="ButtonSize.Regular"/>.
        /// </summary>
        public ButtonSize Size { get; set; } = ButtonSize.Regular;
    }
}
