using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {
    
    public class ButtonFrameworkEditor : IFrameworkEditor {

        private struct Button {
            public MethodInfo Method;
            public ButtonAttribute Attribute;

            public string Name => Attribute.Name ?? Method.Name;
        }

        private readonly List<Button> ButtonsToDraw = new List<Button>();

        public void OnEnable(IEnumerable<Object> targets) {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic;
            var methods = targets.First().GetType().GetMethods(flags);

            foreach (MethodInfo method in methods) {
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                if (buttonAttribute == null)
                    continue;

                ButtonsToDraw.Add(new Button {Method = method, Attribute = buttonAttribute});
            }
        }

        public void OnInspectorGUIBefore(IEnumerable<Object> targets) { }

        public void OnInspectorGUIAfter(IEnumerable<Object> targets) {

            foreach (var button in ButtonsToDraw) {
                if (GUILayout.Button(button.Name, GetLayoutOptions(button))) {
                    foreach (var target in targets) {
                        button.Method.Invoke(target, new object[0]);
                    }
                }
            }
        }

        private GUILayoutOption[] GetLayoutOptions(Button button) {
            GUILayoutOption heightLayoutOption;
            switch (button.Attribute.Size) {
                case ButtonSize.Small:
                    heightLayoutOption = GUILayout.Height(20);
                    break;
                default:
                case ButtonSize.Regular:
                    heightLayoutOption = GUILayout.Height(30);
                    break;
                case ButtonSize.Large:
                    heightLayoutOption = GUILayout.Height(45);
                    break;
                case ButtonSize.ExtraLarge:
                    heightLayoutOption = GUILayout.Height(60);
                    break;
            }

            return new[] {heightLayoutOption};
        }
    }
}