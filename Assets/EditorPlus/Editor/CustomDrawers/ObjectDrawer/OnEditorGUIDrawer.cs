using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace EditorPlus.Editor {
    public class OnEditorGUIDrawer : IFrameworkEditor {

        private List<Action> EditorCallbacks = new List<Action>();
        public void OnEnable(List<Object> targets) {
            foreach (var target in targets) {
                EditorCallbacks.AddRange(GetEditorCallbacks(target));
            }
        }

        private List<Action> GetEditorCallbacks(Object obj) {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic;

            List<MethodInfo> methods = obj.GetType().GetMethods(flags)
                .Where(methodInfo => methodInfo.GetCustomAttribute<OnEditorGUIAttribute>() != null).ToList();

            for (int i = methods.Count - 1; i >= 0; i--) {
                if (!IsSuitableForEditorGUI(methods[i])) {
                    Debug.LogError($"Method {methods[i].Name} got the OnEditorGUI attribute, while not suitable for it.");
                    methods.RemoveAt(i);
                }
            }
            
            return methods.Select(methodInfo => (Action)methodInfo.CreateDelegate(typeof(Action), obj))
                .ToList();
        }

        private bool IsSuitableForEditorGUI(MethodInfo method) {
            return !method.IsConstructor && method.GetParameters().Length == 0;
        }

        public void OnInspectorGUIBefore(List<Object> targets) 
        { }

        public void OnInspectorGUIAfter(List<Object> targets) {
            foreach (var editorCallback in EditorCallbacks) {
                editorCallback.Invoke();
            }
        }
    }
}
