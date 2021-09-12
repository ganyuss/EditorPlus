using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;


namespace EditorPlus.Editor {
    public class OnEditorGUIDrawer : IFrameworkEditor {

        private List<Action> EditorCallbacks = new List<Action>();
        public void OnEnable(IEnumerable<Object> targets) {
            foreach (var target in targets) {
                EditorCallbacks.AddRange(GetEditorCallbacks(target));
            }
        }

        private List<Action> GetEditorCallbacks(Object obj) {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic;

            return obj.GetType().GetMethods(flags)
                .Where(methodInfo => methodInfo.GetCustomAttribute<OnEditorGUIAttribute>() != null)
                .Select(methodInfo => (Action)methodInfo.CreateDelegate(typeof(Action), obj))
                .ToList();
        }

        public void OnInspectorGUIBefore(IEnumerable<Object> targets) 
        { }

        public void OnInspectorGUIAfter(IEnumerable<Object> targets) {
            foreach (var editorCallback in EditorCallbacks) {
                editorCallback.Invoke();
            }
        }
    }
}
