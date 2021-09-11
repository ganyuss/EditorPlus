using UnityEngine;
using System;
using JetBrains.Annotations;
using UnityEditor;

namespace EditorPlus.Editor {

    /// <summary>
    /// This class is the base class of the <see cref="EditorPlus.Editor.DrawerBase&lt;Attr&gt;"/> class.
    /// It is used as a reference to drawers without taking in account the related attribute type.<br /><br />
    /// <b>DO NOT INHERIT FROM THIS CLASS</b>. If you want to create a custom drawer, inherit from
    /// <c>DrawerBase</c> instead.
    /// </summary>
    public abstract class Drawer {
        public abstract Type AttributeType { get; }

        public virtual bool ShowProperty => true;

        public abstract float GetHeight(SerializedProperty property, GUIContent label);

        public abstract Rect OnGUI(Rect position, SerializedProperty property, GUIContent label);

        public abstract void SetAttribute(Attribute attribute);
    }
    
    /// <summary>
    /// Override this class to create a custom drawer. Decorators are objects used to
    /// draw a field editor.
    /// </summary>
    /// <typeparam name="Attr">The attribute used to signal on what field to draw</typeparam>
    public abstract class DrawerBase<Attr> : Drawer where Attr : PropertyAttribute {
        public override Type AttributeType => typeof(Attr);
        public Attr CurrentAttribute;
        
        
        public override void SetAttribute(Attribute attribute) {
            if (attribute.GetType() == AttributeType)
                CurrentAttribute = (Attr) attribute;
        }
    }
}
