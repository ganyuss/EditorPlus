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
        public string TargetPropertyPath { set; }
        public void OnEnable(List<object> targets);
        public float GetHeight(List<object> targets);
        public Rect OnInspectorGUIBefore(Rect rect, List<object> targets);
        public Rect OnInspectorGUIAfter(Rect rect, List<object> targets);

    }
    
#if EDITOR_PLUS_CUSTOM_EDITOR
    
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class CustomObjectEditor : UnityEditor.Editor {
        private List<IClassDecorator> Editors;

        private SerializedPropertyDrawerList DrawerList = new SerializedPropertyDrawerList();

        private void OnEnable() {

        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            
            SerializedPropertyDrawer drawer = DrawerList.GetDrawer(property);
            Rect controlRect = EditorGUILayout.GetControlRect(true, drawer.GetPropertyHeight(false));
            drawer.Draw(controlRect, false);

            serializedObject.ApplyModifiedProperties();
        }


    }

    public class SerializedPropertyDrawerList {
        private Dictionary<string, SerializedPropertyDrawer> DrawerCache = new Dictionary<string, SerializedPropertyDrawer>();

        public SerializedPropertyDrawer GetDrawer(SerializedProperty property) {
            if (DrawerCache.TryGetValue(property.propertyPath, out var Drawer)) {
                Drawer.Property = property;
                return Drawer;
            }

            SerializedPropertyDrawer newDrawer = new SerializedPropertyDrawer(property.Copy(), this);
            DrawerCache[property.propertyPath] = newDrawer;
            return newDrawer;
        }
    }
    
    public class SerializedPropertyDrawer {

        public SerializedProperty Property;
        
        private SerializedPropertyDrawerList DrawerList;
        private Dictionary<string, ListDrawer> _listDrawers = new Dictionary<string, ListDrawer>();
        private readonly float FieldMargin = EditorGUIUtility.standardVerticalSpacing;
        private List<IClassDecorator> _classDecoratorList;
        private List<object> _targetList;

        private List<IClassDecorator> GetClassDecorators() {
            if (_classDecoratorList != null)
                return _classDecoratorList;

            _classDecoratorList = TypeUtils.GetAllTypesImplementing(typeof(IClassDecorator))
                .Select(TypeUtils.CreateInstance<IClassDecorator>)
                .ToList();
            
            List<object> targetList = GetTargetList();
            
            foreach (var classDecorator in _classDecoratorList) {
                classDecorator.TargetPropertyPath = Property.propertyPath;
                classDecorator.OnEnable(targetList);
            }
            
            return _classDecoratorList;
        }

        private List<object> GetTargetList() {
            if (_targetList != null)
                return _targetList;
            
            _targetList = new List<object>();

            if (string.IsNullOrEmpty(Property.propertyPath)) {
                _targetList = Property.serializedObject.targetObjects.Select(obj => (object) obj).ToList();
            }
            else {
                foreach (var masterParentTarget in Property.serializedObject.targetObjects) {
                    EditorUtils.GetMemberInfo(masterParentTarget, Property, out var parentObject, out var parentMemberInfo);
                    _targetList.Add(EditorUtils.GetGeneralValue<object>(parentObject, parentMemberInfo));
                }
            }


            return _targetList;
        }

        
        public SerializedPropertyDrawer(SerializedProperty property, SerializedPropertyDrawerList drawerList) {
            Property = property;
            DrawerList = drawerList;
        }
        
        private ListDrawer GetListDrawer(SerializedProperty property) {
            if (_listDrawers.TryGetValue(property.propertyPath, out var value)) {
                return value;
            }

            _listDrawers[property.propertyPath] = new ListDrawer(property, DrawerList);
            return _listDrawers[property.propertyPath];
        }

        public float GetPropertyHeight(bool showLabel = true) {
            float fieldHeight;
            if (IsPropertyToDrawAsDefault(Property))
                fieldHeight = EditorGUI.GetPropertyHeight(Property, showLabel ? new GUIContent(Property.displayName) : GUIContent.none);
            else if (Property.isArray) {
                fieldHeight = GetListDrawer(Property).GetHeight(Property);
            }
            else {
                fieldHeight = showLabel ? EditorGUIUtility.singleLineHeight : 0;

                SerializedProperty nextProperty = Property.Copy();
                int startDepth = Property.depth;
                if (nextProperty.NextVisible(true)) {
                    do {
                        fieldHeight += DrawerList.GetDrawer(nextProperty).GetPropertyHeight();
                    } while (nextProperty.NextVisible(false) && startDepth < nextProperty.depth);
                }
                
                List<IClassDecorator> decorators = GetClassDecorators();
                List<object> targets = GetTargetList();

                fieldHeight += decorators.Select(d => d.GetHeight(targets)).Sum();
            }

            return fieldHeight > 0 ? fieldHeight + 2 * FieldMargin : 0;
        }

        public void Draw(Rect rect, bool showLabel = true) {
            if (rect.height != 0) {
                rect.height -= FieldMargin * 2;
                rect.y += FieldMargin;
            }

            if (IsPropertyToDrawAsDefault(Property)) {
                EditorGUI.PropertyField(rect, Property, showLabel ? new GUIContent(Property.displayName) : GUIContent.none);
            }
            else if (Property.isArray) {
                DrawList(Property, rect);
            }
            // Custom class without custom attributes, we can draw it as Unity would do it.
            else {
                if (showLabel) {
                    Rect labelRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
                    Property.isExpanded = EditorGUI.Foldout(labelRect, Property.isExpanded, Property.displayName);

                    rect.ToBottomOf(labelRect);
                    EditorGUI.indentLevel += 1;
                }
                else {
                    Property.isExpanded = true;
                }

                if (Property.isExpanded) {
                    
                    List<IClassDecorator> decorators = GetClassDecorators();
                    List<object> targets = GetTargetList();
                    
                    foreach (var decorator in decorators) {
                        rect = decorator.OnInspectorGUIBefore(rect, targets);
                    }
                    
                    SerializedProperty nextProperty = Property.Copy();
                    int startDepth = Property.depth;
                    if (nextProperty.NextVisible(true)) {
                        do {
                            SerializedPropertyDrawer drawer = DrawerList.GetDrawer(nextProperty);
                            Rect propertyRect = new Rect(rect) { height = drawer.GetPropertyHeight() }; 
                            drawer.Draw(propertyRect);
                            rect.ToBottomOf(propertyRect);
                        } while (nextProperty.NextVisible(false) && startDepth < nextProperty.depth);
                    }
                    
                    foreach (var decorator in decorators) {
                        rect = decorator.OnInspectorGUIAfter(rect, targets);
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
            
            public ListDrawer(SerializedProperty property, SerializedPropertyDrawerList drawerList) {
                
                EditorUtils.GetMemberInfo(property, out _, out var targetMemberInfo);
                Decorators = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(targetMemberInfo);
                DecoratorsReversed = ((IEnumerable<Decorator>)Decorators).Reverse().ToList();
                
                ActualDrawer = new ListPropertyDrawer(property, drawerList);
            }

            public float GetHeight(SerializedProperty property) {
                return Decorators.Select(d => d.GetHeight(property, null)).Sum()
                       + ActualDrawer.GetHeight(property, null);
            }
            
            public void Draw(SerializedProperty property, Rect rect) {
                rect.x += EditorGUI.indentLevel * 10;
                rect.width -= EditorGUI.indentLevel * 10;

                foreach (var decorator in Decorators) {
                    rect = decorator.OnBeforeGUI(rect, property);
                }

                rect = ActualDrawer.OnGUI(rect, property, null);
                
                foreach (var decorator in DecoratorsReversed) {
                    rect = decorator.OnAfterGUI(rect, property);
                }
                
            }
        }
    }
#endif
}
