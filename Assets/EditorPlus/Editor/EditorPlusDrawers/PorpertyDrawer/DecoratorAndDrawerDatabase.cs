using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// This class is basically a factory for all the different attribute drawers and decorators
    /// created using EditorPlus.
    /// </summary>
    [InitializeOnLoad]
    public static class DecoratorAndDrawerDatabase {

        private static Dictionary<Type, Func<Decorator>> DecoratorFactoryDictionary;
        private static Dictionary<Type, Func<AttributeDrawer>> AttributeDrawerFactoryDictionary;
        private static Dictionary<Type, Func<PropertyDrawer>> PropertyDrawerFactoryDictionary;

        /// <summary>
        /// Return all the different decorator attribute type registered.
        /// </summary>
        /// <returns>all the attributes used in a <see cref="DecoratorBase&lt;Attr&gt;"/>
        /// throughout the project.</returns>
        public static Type[] GetAllDecoratorAttributeTypes() {
            return DecoratorFactoryDictionary.Keys.ToArray();
        }
        
        /// <summary>
        /// Return all the different drawer attribute type registered.
        /// </summary>
        /// <returns>all the attributes used in a <see cref="AttributeDrawerBase&lt;Attr&gt;"/>
        /// throughout the project.</returns>
        public static Type[] GetAllDrawerTypes() {
            return AttributeDrawerFactoryDictionary.Keys.Concat(PropertyDrawerFactoryDictionary.Keys).ToArray();
        }

        /// <summary>
        /// If the given type is a decorator attribute, returns true and sets decorator
        /// to an instance of the decorator class associated with the given decorator attribute type.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <param name="decorator">Set to a decorator instance associated to the attribute type, if
        /// the attribute is a decorator attribute.</param>
        /// <returns>True if the given type is an decorator attribute, otherwise false.</returns>
        private static bool TryGetDecoratorFor(Type attributeType, out Decorator decorator) {
            bool ok = DecoratorFactoryDictionary.TryGetValue(attributeType, out var factory);
            decorator = factory?.Invoke();
            return ok;
        }
        
        /// <summary>
        /// If the given type is a drawer attribute, returns true and sets drawer
        /// to an instance of the drawer class associated with the given drawer attribute type.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <param name="drawer">Set to a drawer instance associated to the attribute type, if
        /// the attribute is a drawer attribute.</param>
        /// <returns>True if the given type is an drawer attribute, otherwise false.</returns>
        private static bool TryGetAttributeDrawerFor(Type attributeType, out AttributeDrawer drawer) {
            bool ok = AttributeDrawerFactoryDictionary.TryGetValue(attributeType, out var factory);
            drawer = factory?.Invoke();
            return ok;
        }
        
        /// <summary>
        /// If the given type has an associated property drawer, returns true and sets drawer
        /// to an instance of the drawer class.
        /// </summary>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="drawer">Set to a drawer instance associated to the property type, if
        /// it exists.</param>
        /// <returns>True if the given type has an associated property drawer, otherwise false.</returns>
        private static bool TryGetPropertyDrawerFor(Type propertyType, out Drawer drawer) {
            bool ok = PropertyDrawerFactoryDictionary.TryGetValue(propertyType, out var factory);
            drawer = factory?.Invoke();
            return ok;
        }

        /// <summary>
        /// Returns a list of decorators associated with a given member.
        /// </summary>
        /// <param name="member">The member to get the decorator to decorate around.</param>
        /// <returns>The decorators associated with the members.</returns>
        public static List<Decorator> GetAllDecoratorsFor(MemberInfo member) {
            List<Decorator> result = new List<Decorator>();
            if (member is null)
                return result;
            
            foreach (var attribute in member.GetCustomAttributes()) {
                if (TryGetDecoratorFor(attribute.GetType(), out var decorator)) {
                    decorator.SetAttribute(attribute);
                    result.Add(decorator);
                }
            }

            result.Sort((d1, d2) => d1.Order - d2.Order);
            return result;
        }

        public static Drawer GetDrawerFor(FieldInfo fieldInfo)
        {
            if (fieldInfo is null)
                return null;
            
            foreach (var attribute in fieldInfo.GetCustomAttributes()) {
                if (TryGetAttributeDrawerFor(attribute.GetType(), out var attributeDrawer))
                {
                    attributeDrawer.SetAttribute(attribute);
                    return attributeDrawer;
                }
            }

            if (TryGetPropertyDrawerFor(fieldInfo.FieldType, out var propertyDrawer))
                return propertyDrawer;

            return new DefaultDrawer();
        }
        
        
        static DecoratorAndDrawerDatabase() {
            DecoratorFactoryDictionary = new Dictionary<Type, Func<Decorator>>();
            AttributeDrawerFactoryDictionary = new Dictionary<Type, Func<AttributeDrawer>>();
            PropertyDrawerFactoryDictionary = new Dictionary<Type, Func<PropertyDrawer>>();
            
            Type[] DecoratorTypes = TypeUtils.GetAllTypesInheritingFrom(typeof(DecoratorBase<>));
            foreach (var decoratorType in DecoratorTypes) {
                if (IsInstantiationValid(decoratorType)) {
                    Decorator decorator = TypeUtils.CreateInstance<Decorator>(decoratorType);
                    DecoratorFactoryDictionary[decorator.AttributeType] = () => TypeUtils.CreateInstance<Decorator>(decoratorType);
                }
            }
            
            Type[] attributeDrawerTypes = TypeUtils.GetAllTypesInheritingFrom(typeof(AttributeDrawerBase<>));
            foreach (var attributeDrawerType in attributeDrawerTypes) {
                if (IsInstantiationValid(attributeDrawerType)) {
                    AttributeDrawer drawer = TypeUtils.CreateInstance<AttributeDrawer>(attributeDrawerType);
                    AttributeDrawerFactoryDictionary[drawer.AttributeType] = () => TypeUtils.CreateInstance<AttributeDrawer>(attributeDrawerType);
                }
            }
            
            Type[] propertyDrawerTypes = TypeUtils.GetAllTypesInheritingFrom(typeof(PropertyDrawerBase<>));
            foreach (var propertyDrawerType in propertyDrawerTypes) {
                if (IsInstantiationValid(propertyDrawerType)) {
                    PropertyDrawer drawer = TypeUtils.CreateInstance<PropertyDrawer>(propertyDrawerType);
                    PropertyDrawerFactoryDictionary[drawer.TargetType] = () => TypeUtils.CreateInstance<PropertyDrawer>(propertyDrawerType);
                }
            }
        }

        private static bool IsInstantiationValid(Type type) {
            return !type.IsGenericType && !type.IsAbstract;
        }
    }
}
