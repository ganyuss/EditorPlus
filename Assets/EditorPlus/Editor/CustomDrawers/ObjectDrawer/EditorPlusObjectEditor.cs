using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {

    /// <summary>
    /// By implementing this interface, one can add a decorator to any class.
    /// It allows you to add elements before or after objects in the inspector.
    /// <br /><br />
    /// it will be used both on the editor target (<see cref="ScriptableObject"/>,
    /// <see cref="MonoBehaviour"/> etc.), but also around object fields.
    /// </summary>
    /// <seealso cref="DecoratorBase&lt;Attr&gt;"/>
    public interface IClassDecorator {
        public string TargetPropertyPath { set; }
        public void OnEnable(List<object> targets);
        public float GetHeight(List<object> targets);
        public Rect OnInspectorGUIBefore(Rect rect, List<object> targets);
        public Rect OnInspectorGUIAfter(Rect rect, List<object> targets);

    }
    
#if !EDITOR_PLUS_DISABLE_EDITOR
    /// <summary>
    /// This class is the main object editor for the plugin. It draws
    /// everything using the <see cref="SerializedPropertyDrawer" />.
    /// </summary>
    /// <seealso cref="SerializedPropertyDrawerList"/>
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class EditorPlusObjectEditor : UnityEditor.Editor {

        private SerializedPropertyDrawerList DrawerList = new SerializedPropertyDrawerList();

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            
            SerializedPropertyDrawer drawer = DrawerList.GetDrawer(property);
            Rect controlRect = EditorGUILayout.GetControlRect(true, drawer.GetPropertyHeight(false));
            drawer.Draw(controlRect, false);

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif

    /// <summary>
    /// This list holds all the <see cref="SerializedPropertyDrawer"/> instantiated by an editor.
    /// It allows for efficient instantiating of the drawers.
    /// <br /><br />
    /// The drawers are identified using the associated property path.
    /// The <see cref="SerializedProperty"/> reference is set on every draw.
    /// </summary>
    public class SerializedPropertyDrawerList {
        private readonly Dictionary<string, SerializedPropertyDrawer> DrawerCache = new Dictionary<string, SerializedPropertyDrawer>();

        /// <summary>
        /// Use this method to get a <see cref="SerializedPropertyDrawer"/> associated
        /// with the property. Multiple calls to this methods with properties with the
        /// same <see cref="SerializedProperty.propertyPath">property path</see> will
        /// return the same drawer.
        /// </summary>
        /// <param name="property">The property to get the drawer for</param>
        /// <returns>A drawer to draw the property.</returns>
        public SerializedPropertyDrawer GetDrawer(SerializedProperty property) {
            if (DrawerCache.TryGetValue(property.propertyPath, out var Drawer)) {
                Drawer.Property = property.Copy();
                return Drawer;
            }

            SerializedPropertyDrawer newDrawer = new SerializedPropertyDrawer(property.Copy(), this);
            DrawerCache[property.propertyPath] = newDrawer;
            return newDrawer;
        }
    }
    
    /// <summary>
    /// This class is responsible for drawing a <see cref="SerializedProperty"/>.
    /// There are 3 different cases:
    /// <ul><li>
    /// The property describes a field that is a generic object, like an object of
    /// a custom <see cref="SerializableAttribute">serializable</see> class. The drawer will draw
    /// the foldout, display decoration from <see cref="IClassDecorator"/> classes,
    /// and display each sub field using other property drawers.
    /// </li><li>
    /// The property describes a list. In that case it will use a <see cref="SerializedPropertyDrawer.ListDrawer"/>
    /// to display it.
    /// </li><li>
    /// Otherwise, the field will be displayed using the
    /// <see cref="EditorGUI.PropertyField(Rect, SerializedProperty, GUIContent)"/> method.
    /// this can lead to an indirect use of the <see cref="EditorPlusPropertyDrawer"/> class.
    /// </li></ul>
    /// </summary>
    public class SerializedPropertyDrawer {

        public SerializedProperty Property;
        
        private readonly SerializedPropertyDrawerList DrawerList;
        private readonly float FieldMargin = EditorGUIUtility.standardVerticalSpacing;
        private ListDrawer ListDrawerInstance;
        private List<IClassDecorator> _classDecoratorList;
        private List<object> _targetList;
        
        public SerializedPropertyDrawer(SerializedProperty property, SerializedPropertyDrawerList drawerList) {
            Property = property;
            DrawerList = drawerList;
        }

        private List<IClassDecorator> GetClassDecorators() {
            if (_classDecoratorList != null)
                return _classDecoratorList;

            _classDecoratorList = TypeUtils.GetAllTypesImplementing(typeof(IClassDecorator))
                .Select(TypeUtils.CreateInstance<IClassDecorator>)
                .ToList();
            
            List<object> targetList = GetTargets();
            
            foreach (var classDecorator in _classDecoratorList) {
                classDecorator.TargetPropertyPath = Property.propertyPath;
                classDecorator.OnEnable(targetList);
            }
            
            return _classDecoratorList;
        }

        private List<object> GetTargets() {
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

        private ListDrawer GetListDrawer(SerializedProperty property) {
            if (ListDrawerInstance is null) {
                ListDrawerInstance = new ListDrawer(property, DrawerList);
            }

            return ListDrawerInstance;
        }

        public float GetPropertyHeight(bool showLabel = true) {
            float fieldHeight;
            if (IsPropertyToDrawAsDefault(Property)) {
                fieldHeight = EditorGUI.GetPropertyHeight(Property,
                    showLabel ? new GUIContent(Property.displayName) : GUIContent.none);
            }
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
                List<object> targets = GetTargets();

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
                if (Property.name == "m_Script")
                    GUI.enabled = false;
                EditorGUI.PropertyField(rect, Property, showLabel ? new GUIContent(Property.displayName) : GUIContent.none);
                if (Property.name == "m_Script")
                    GUI.enabled = true;
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
                    List<object> targets = GetTargets();
                    
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
        
        /// <summary>
        /// This class is used to draw a list in the unity editor, a bit differently than
        /// the regular editor list. Especially, this class uses the <see cref="ListPropertyDrawer"/>
        /// class, which takes in account the <see cref="BetterListAttribute">BetterList attribute</see>.
        /// </summary>
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
                return Decorators.Select(d => d.GetHeight(property)).Sum()
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
}
