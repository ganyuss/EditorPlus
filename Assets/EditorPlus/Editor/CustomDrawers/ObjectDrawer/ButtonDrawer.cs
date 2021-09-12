using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {
    
    public class ButtonDrawer : IFrameworkEditor {

        private struct Button {
            public Action Action;
            public ButtonAttribute Attribute;
            public List<Decorator> Decorators;
            public string Name;

            public bool Equals(Button other) {
                return Name == other.Name
                    && Attribute.Size == other.Attribute.Size
                    && Decorators.Select(d => d.GetType()).SequenceEqual(other.Decorators.Select(d => d.GetType()));
            }

            public void Merge(Button other) {
                Action += other.Action;
            }
        }

        private readonly List<Button> ButtonsToDraw = new List<Button>();

        public void OnEnable(List<Object> targets) {

            foreach (var target in targets) {
                List<MethodInfo> methods = GetAllButtonMethods(target);
                
                foreach (MethodInfo method in methods) {

                    var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                    if (buttonAttribute == null)
                        continue;

                    if (!IsSuitableForButton(method)) {
                        Debug.LogError(
                            $"Method \"{method.Name}\" got Button attribute, while not suitable for button calls");
                        continue;
                    }

                    ButtonsToDraw.Add(new Button {
                        Name = buttonAttribute.Name ?? method.Name,
                        Attribute = buttonAttribute,
                        Decorators = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(method),
                        Action = (Action) method.CreateDelegate(typeof(Action), target)
                    });
                }

                for (int i = 0; i < ButtonsToDraw.Count; i++) {
                    Button currentButton = ButtonsToDraw[i];

                    for (int j = ButtonsToDraw.Count - 1; j > i; j--) {
                        if (currentButton.Equals(ButtonsToDraw[j])) {
                            currentButton.Merge(ButtonsToDraw[j]);
                        }
                        
                        ButtonsToDraw.RemoveAt(j);
                    }
                }
            }
        }

        private bool IsSuitableForButton(MethodInfo method) {
            return !method.IsAbstract && !method.IsConstructor && method.GetParameters().Length == 0;
        }

        private List<MethodInfo> GetAllButtonMethods(Object target) {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic;
            return target.GetType().GetMethods(flags)
                .Where(method => method.GetCustomAttribute<ButtonAttribute>() != null)
                .ToList();
        } 

        public void OnInspectorGUIBefore(List<Object> targets) { }

        public void OnInspectorGUIAfter(List<Object> targets) {

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
                button.Action.Invoke();
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