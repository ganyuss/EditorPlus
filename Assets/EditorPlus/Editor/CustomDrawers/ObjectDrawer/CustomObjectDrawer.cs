using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {

    public interface IClassDecorator {
        public void OnEnable(List<Object> targets);
        public void OnInspectorGUIBefore(List<Object> targets);
        public void OnInspectorGUIAfter(List<Object> targets);
    }
    
#if EDITOR_PLUS_CUSTOM_EDITOR
    
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class CustomObjectEditor : UnityEditor.Editor {
        private List<IClassDecorator> Editors;
        private readonly SerializedFieldDrawer _serializedFieldDrawer = new SerializedFieldDrawer();

        private void OnEnable() {
            Editors = TypeUtils.GetAllTypesImplementing(typeof(IClassDecorator))
                .Select(TypeUtils.CreateInstance<IClassDecorator>)
                .ToList();
            
            List<Object> targetList = targets.ToList();
            
            foreach (var editor in Editors) {
                editor.OnEnable(targetList);
            }
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            List<Object> targetList = targets.ToList();
            
            foreach (var editor in Editors) {
                editor.OnInspectorGUIBefore(targetList);
            }

            SerializedProperty firstProperty = serializedObject.GetIterator();
            SerializedProperty secondProperty = firstProperty.Copy();
            if (secondProperty.NextVisible(true)) {
                float totalHeight = _serializedFieldDrawer.GetPropertyHeight(firstProperty.Copy());
                Rect controlRect = EditorGUILayout.GetControlRect(true, totalHeight);
                _serializedFieldDrawer.DrawAllWithChildren(secondProperty, controlRect);
            }

            foreach (var editor in Editors) {
                editor.OnInspectorGUIAfter(targetList);
            }

            serializedObject.ApplyModifiedProperties();
        }


    }
    
    public class SerializedFieldDrawer {
        private Dictionary<string, ListDrawer> _listDrawers = new Dictionary<string, ListDrawer>();
        private readonly float FieldMargin = EditorGUIUtility.standardVerticalSpacing;

        private ListDrawer GetListDrawer(SerializedProperty property) {
            if (_listDrawers.TryGetValue(property.propertyPath, out var value)) {
                return value;
            }
            _listDrawers[property.propertyPath] = new ListDrawer(property);
            return _listDrawers[property.propertyPath];
        }

        public void DrawAllWithChildren(SerializedProperty property, Rect rect, bool depthLimit = false) {

            int currentDepth = property.depth;
            do {
                Rect currentRect = new Rect(rect) {height = GetPropertyHeight(property.Copy())};
                Draw(property.Copy(), currentRect);
                rect.ToBottomOf(currentRect);
            } while (property.NextVisible(false) && (!depthLimit || currentDepth <= property.depth));
        }

        public float GetPropertyHeight(SerializedProperty property, bool showLabel = true) {
            float fieldHeight;
            if (IsPropertyToDrawAsDefault(property))
                fieldHeight = EditorGUI.GetPropertyHeight(property, showLabel ? new GUIContent(property.displayName) : GUIContent.none);
            else if (property.isArray) {
                fieldHeight = GetListDrawer(property).GetHeight(property);
            }
            else {
                fieldHeight = showLabel ? EditorGUIUtility.singleLineHeight : 0;

                property.NextVisible(true);
                int currentDepth = property.depth;
                do {
                    fieldHeight += GetPropertyHeight(property.Copy());
                } while (property.NextVisible(false) && currentDepth <= property.depth);
            }

            return fieldHeight > 0 ? fieldHeight + 2 * FieldMargin : 0;
        }

        public void Draw(SerializedProperty property, Rect rect, bool showLabel = true) {
            if (rect.height != 0) {
                rect.height -= FieldMargin * 2;
                rect.y += FieldMargin;
            }


            if (IsPropertyToDrawAsDefault(property)) {
                EditorGUI.PropertyField(rect, property, showLabel ? new GUIContent(property.displayName) : GUIContent.none);
            }
            else if (property.isArray) {
                DrawList(property, rect);
            }
            // Custom class without custom attributes, we can draw it as Unity would do it.
            else {
                if (showLabel) {
                    Rect labelRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
                    property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, property.displayName);

                    rect.ToBottomOf(labelRect);
                    EditorGUI.indentLevel += 1;
                }
                else {
                    property.isExpanded = true;
                }

                if (property.isExpanded) {
                    if (property.NextVisible(true)) {
                        DrawAllWithChildren(property, rect, true);
                    }
                }

                if (showLabel) {
                    EditorGUI.indentLevel -= 1;
                }
            }
        }

        private bool IsPropertyToDrawAsDefault(SerializedProperty property) =>
            property.propertyType != SerializedPropertyType.Generic 
            || !property.isArray && HasCustomAttributes(property);

        private bool HasCustomAttributes(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, out _, out var targetMemberInfo);

            List<Attribute> customAttributes = targetMemberInfo?.GetCustomAttributes().ToList();
            return targetMemberInfo != null && (customAttributes.Count > 2 || customAttributes.Count > 1 && customAttributes[0].GetType() != typeof(SerializeField));
        }

        private void DrawList(SerializedProperty property, Rect rect) {
            GetListDrawer(property).Draw(property, rect);
        }
        
        private class ListDrawer {
            List<Decorator> Decorators;
            List<Decorator> DecoratorsReversed;
            private ListPropertyDrawer ActualDrawer;
            
            public ListDrawer(SerializedProperty property) {
                
                EditorUtils.GetMemberInfo(property, out _, out var targetMemberInfo);
                Decorators = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(targetMemberInfo);
                DecoratorsReversed = ((IEnumerable<Decorator>)Decorators).Reverse().ToList();
                
                ActualDrawer = new ListPropertyDrawer(property);
            }

            public float GetHeight(SerializedProperty property) {
                return Decorators.Select(d => d.GetHeight(property, null)).Sum()
                       + ActualDrawer.GetHeight(property, null);
            }
            
            public void Draw(SerializedProperty property, Rect rect) {
                rect.x += EditorGUI.indentLevel * 10;
                rect.width -= EditorGUI.indentLevel * 10;

                foreach (var decorator in Decorators) {
                    rect = decorator.OnBeforeGUI(rect, property, null);
                }

                rect = ActualDrawer.OnGUI(rect, property, null);
                
                foreach (var decorator in DecoratorsReversed) {
                    rect = decorator.OnAfterGUI(rect, property, null);
                }
                
            }
        }
    }
#endif
}
