using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// This class is the interface between the custom EditorPlus drawer system and the Unity system.
    /// <seealso cref="AttributeDrawerBase{Attr}"/>
    /// <seealso cref="DecoratorBase&lt;Attr&gt;"/>
    /// </summary>
    public partial class CustomUnityDrawer : PropertyDrawer {

        private List<Decorator> _decoratorsToUse;
        private List<Decorator> _decoratorsToUseReversed;
        private Drawer _propertyDrawer;

        private List<Decorator> GetDecoratorsToUse(SerializedProperty property) {
            if (_decoratorsToUse != null)
                return _decoratorsToUse;
            
            _decoratorsToUse = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(property.GetFieldInfo());

            return _decoratorsToUse;
        }

        private List<Decorator> GetDecoratorsToUseReversed(SerializedProperty property) {
            if (_decoratorsToUseReversed != null)
                return _decoratorsToUseReversed;
            
            _decoratorsToUseReversed = ((IEnumerable<Decorator>)GetDecoratorsToUse(property)).Reverse().ToList();
            return _decoratorsToUseReversed;
        }
        
        private Drawer GetPropertyDrawer(SerializedProperty property) {
            if (_propertyDrawer != null)
                return _propertyDrawer;

            foreach (var drawerAttributeType in DecoratorAndDrawerDatabase.GetAllDrawerAttributeTypes()) {
                if (HasAttribute(property, drawerAttributeType, out var currentAttribute)) {
                    AttributeDrawer drawer = DecoratorAndDrawerDatabase.GetDrawerFor(drawerAttributeType);
                    drawer.SetAttribute(currentAttribute);
                    return drawer;
                }
            }

            _propertyDrawer = new DefaultDrawer();
            return _propertyDrawer;
        }
        

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            List<Decorator> decorators = GetDecoratorsToUse(property);

            if (!decorators.All(decorator => decorator.ShowProperty))
                return 0;
            
            float height = 0;
            foreach (var decorator in decorators) {
                height += decorator.GetHeight(property, label);
            }

            float propertyHeight = GetPropertyDrawer(property).GetHeight(property, label);
            return height + propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            List<Decorator> decorators = GetDecoratorsToUse(property);
            List<Decorator> decoratorsReversed = GetDecoratorsToUseReversed(property);
            
            if (!decorators.All(decorator => decorator.ShowProperty))
                return;
            
            foreach (var decorator in decorators) {
                position = decorator.OnBeforeGUI(position, property, label);
            }

            if (decorators.All(decorator => decorator.ShowProperty)) {
                position = GetPropertyDrawer(property).OnGUI(position, property, label);
            }
            
            // We want the first decorator to be the last one called here
            foreach (var decorator in decoratorsReversed) {
                position = decorator.OnAfterGUI(position, property, label);
            }
        }
        
        
        private static bool HasAttribute(SerializedProperty property, Type attributeType, out Attribute attribute)
        {
            FieldInfo fi = property.GetFieldInfo();
            attribute = fi?.GetCustomAttribute(attributeType);
            return attribute != null;
        }
    }
    
    public class DefaultPropertyAttribute : PropertyAttribute { }

    public class DefaultDrawer : AttributeDrawerBase<DefaultPropertyAttribute> {
        
        public override float GetHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        
        public override Rect OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            float height = GetHeight(property, label);
            Rect propertyRect = new Rect(position) {height = height};
            
            label.text ??= property.displayName;
            EditorGUI.PropertyField(propertyRect, property, label);
            
            position.ToBottomOf(propertyRect);
            return position;
        }
    }
}