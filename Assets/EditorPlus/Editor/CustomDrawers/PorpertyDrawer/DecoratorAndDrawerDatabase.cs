using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    [InitializeOnLoad]
    public static class DecoratorAndDrawerDatabase {

        private static Dictionary<Type, Func<Decorator>> DecoratorFactoryDictionary;
        private static Dictionary<Type, Func<Drawer>> DrawerFactoryDictionary;

        public static Type[] GetAllDecoratorAttributeTypes() {
            return DecoratorFactoryDictionary.Keys.ToArray();
        }
        
        public static Type[] GetAllDrawerAttributeTypes() {
            return DrawerFactoryDictionary.Keys.ToArray();
        }

        public static bool IsDecoratorAttribute(Type attributeType) {
            return DecoratorFactoryDictionary.ContainsKey(attributeType);
        }
        
        public static bool IsDrawerAttribute(Type attributeType) {
            return DrawerFactoryDictionary.ContainsKey(attributeType);
        }

        public static Decorator GetDecoratorFor(Type attributeType) {
            return DecoratorFactoryDictionary[attributeType].Invoke();
        }

        public static Drawer GetDrawerFor(Type attributeType) {
            return DrawerFactoryDictionary[attributeType].Invoke();
        }
        
        public static bool TryGetDecoratorFor(Type attributeType, out Decorator decorator) {
            bool ok = DecoratorFactoryDictionary.TryGetValue(attributeType, out var factory);
            decorator = factory?.Invoke();
            return ok;
        }
        
        public static bool TryGetDrawerFor(Type attributeType, out Drawer drawer) {
            bool ok = DrawerFactoryDictionary.TryGetValue(attributeType, out var factory);
            drawer = factory?.Invoke();
            return ok;
        }

        public static List<Decorator> GetAllDecoratorsFor(MemberInfo member) {
            List<Decorator> result = new List<Decorator>();
            
            foreach (var attribute in member.GetCustomAttributes()) {
                if (TryGetDecoratorFor(attribute.GetType(), out var decorator)) {
                    decorator.SetAttribute(attribute);
                    result.Add(decorator);
                }
            }

            result.Sort((d1, d2) => d1.Order - d2.Order);
            return result;
        }
        
        
        static DecoratorAndDrawerDatabase() {
            DecoratorFactoryDictionary = new Dictionary<Type, Func<Decorator>>();
            DrawerFactoryDictionary = new Dictionary<Type, Func<Drawer>>();
            
            Type[] DecoratorTypes = TypeUtils.GetAllTypesInheritingFrom(typeof(DecoratorBase<>));
            foreach (var decoratorType in DecoratorTypes) {
                if (IsInstantiationValid(decoratorType)) {
                    Decorator decorator = TypeUtils.CreateInstance<Decorator>(decoratorType);
                    DecoratorFactoryDictionary[decorator.AttributeType] = () => TypeUtils.CreateInstance<Decorator>(decoratorType);
                }
            }
            
            Type[] DrawerTypes = TypeUtils.GetAllTypesInheritingFrom(typeof(DrawerBase<>));
            foreach (var drawerType in DrawerTypes) {
                if (IsInstantiationValid(drawerType)) {
                    Drawer drawer = TypeUtils.CreateInstance<Drawer>(drawerType);
                    DrawerFactoryDictionary[drawer.AttributeType] = () => TypeUtils.CreateInstance<Drawer>(drawerType);
                }
            }
        }

        private static bool IsInstantiationValid(Type type) {
            return !type.IsGenericType && !type.IsAbstract;
        }
    }
}
