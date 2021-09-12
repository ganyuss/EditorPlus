using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EditorPlus {
    public static class TypeUtils 
    {
        public static IEnumerable<Type> GetAllTypes() {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        }
    
        public static IEnumerable<Type> GetTypesFromName(string typeName) {
            List<Type> output = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType(typeName);
                if (type != null) output.Add(type);
            }

            return output;
        }
        
        [NotNull]
        public static Type[] GetAllTypesImplementing(Type interfaceType) {

            List<Type> output = new List<Type>();
            //parentType.gene
            
            foreach (Type type in GetAllTypes()) {
                if (type.GetInterfaces().Contains(interfaceType)) {
                    output.Add(type);
                }
            }

            return output.ToArray();
        }
        
        [NotNull]
        public static Type[] GetAllTypesInheritingFrom(Type parentType) {

            List<Type> output = new List<Type>();
            //parentType.gene
            
            foreach (Type type in GetAllTypes()) {
                Type currentType = type;
                while ((currentType = currentType.BaseType) != null) {
                    if (CorrespondTo(parentType, currentType)) {
                        output.Add(type);
                        break;
                    }
                }
            }

            return output.ToArray();
        }

        public static T CreateInstance<T>(Type type) {
            return (T) Activator.CreateInstance(type);
        }

        /// <summary>
        /// Compares two types, without taking in account the generic parameters.
        /// </summary>
        /// <example>
        /// <c>CorrespondTo(typeof(List&lt;&gt;), typeof(List&lt;string&gt;))</c> will return true.<br />
        /// <c>CorrespondTo(typeof(List&lt;&gt;), typeof(string))</c> will return false.
        /// </example>
        /// <param name="type1">The first type to test</param>
        /// <param name="type2">The second type to test the first type against</param>
        /// <returns>True if they describe the same type, whatever the generic parameters.</returns>
        private static bool CorrespondTo(Type type1, Type type2) {
            if (type1.IsEmptyGeneric())
                type1 = type1.GetGenericTypeDefinition();

            if (type2.IsEmptyGeneric())
                type2 = type2.GetGenericTypeDefinition();
            
            return type1 == type2;
        }
        
        private static bool IsEmptyGeneric(this Type type) {
            return type.IsGenericType && ! type.ContainsGenericParameters;
        }
    }
}