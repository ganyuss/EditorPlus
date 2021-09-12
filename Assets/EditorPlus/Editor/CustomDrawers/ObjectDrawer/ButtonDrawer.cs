using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {
    
    public class ButtonDrawer : IFrameworkEditor {

        private struct Button {
            public MethodInfo Method;
            public ButtonAttribute Attribute;
            public List<Decorator> Decorators;

            public string Name => Attribute.Name ?? Method.Name;
        }

        private readonly List<Button> ButtonsToDraw = new List<Button>();

        public void OnEnable(IEnumerable<Object> targets) {
            List<MethodInfo> methods = null;

            foreach (var target in targets) {
                if (methods == null) {
                    methods = GetAllButtonMethods(target);
                }
                else {
                    List<MethodInfo> otherMethods = GetAllButtonMethods(target);
                    methods = methods.Where(method1 => otherMethods.Any(method2 => method1.Name == method2.Name)).ToList();
                }
            }

            if (methods == null) return;

            foreach (MethodInfo method in methods) {
                if (method.IsConstructor) continue;
                
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                if (buttonAttribute == null)
                    continue;

                ButtonsToDraw.Add(new Button {
                    Method = method, 
                    Attribute = buttonAttribute,
                    Decorators = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(method)
                });
            }
        }

        private List<MethodInfo> GetAllButtonMethods(Object target) {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic;
            return target.GetType().GetMethods(flags)
                .Where(method => method.GetCustomAttribute<ButtonAttribute>() != null)
                .ToList();
        } 

        public void OnInspectorGUIBefore(IEnumerable<Object> targets) { }

        public void OnInspectorGUIAfter(IEnumerable<Object> targets) {

            foreach (var button in ButtonsToDraw) {
                Draw(button, targets);
            }
        }

        private void Draw(Button button, IEnumerable<Object> targets) {
            Rect currentRect = EditorGUILayout.GetControlRect(GUILayout.Height(GetHeight(button)));
            
            foreach (var decorator in button.Decorators) {
                currentRect = decorator.OnBeforeGUI(currentRect);
            }

            Rect buttonRect = new Rect(currentRect) {height = GetButtonHeight(button.Attribute.Size)};
            if (GUI.Button(buttonRect, button.Name)) {
                foreach (var target in targets) {
                    button.Method.Invoke(target, new object[0]);
                }
            }
            currentRect.ToBottomOf(buttonRect);
            
            List<Decorator> reversedDecorators = button.Decorators.ToList();
            reversedDecorators.Reverse();
            foreach (var decorator in reversedDecorators) {
                currentRect = decorator.OnBeforeGUI(currentRect);
            }
        }

        private float GetHeight(Button button) {
            float height = button.Decorators.Select(d => d.GetHeight()).Sum();
            height += GetButtonHeight(button.Attribute.Size);

            return height;
        }

        private float GetButtonHeight(ButtonSize size) {
            switch (size) {
                case ButtonSize.Small:
                    return 20;
                default:
                case ButtonSize.Regular:
                    return 30;
                case ButtonSize.Large:
                    return 45;
                case ButtonSize.ExtraLarge:
                    return 60;
            }
        }
    }
}