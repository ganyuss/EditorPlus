using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using EditorPlus.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public class ListPropertyDrawer {
        
        private ReorderableList InnerList;
        private SerializedProperty CurrentProperty;
        private SerializedFieldDrawer FieldDrawer = new SerializedFieldDrawer();

        private bool ShowFocus = true;
        private bool AlwaysExpanded = false;
        
        private bool AddMethodError = false;
        private bool RemoveMethodError = false;
        
        private string AddMethodErrorText = "The method name provided as AddMethod in the BetterList attribute " +
                                            "is incorrect. The name must point to a method without any arguments.";
        private string RemoveMethodErrorText = "The method name provided as RemoveMethod in the BetterList attribute " +
                                            "is incorrect. The name must point to a method with only one argument, either " +
                                            "the index of the element to remove or the element itself.";

        public ListPropertyDrawer(SerializedProperty property) {
            CurrentProperty = property.Copy();
            InnerList = new ReorderableList(CurrentProperty.serializedObject, CurrentProperty) {
                drawHeaderCallback = DrawListHeader,
                drawElementCallback = DrawElement,
                elementHeightCallback = GetElementHeight,
                drawElementBackgroundCallback = DrawElementBackground,
            };
            
            EditorUtils.GetMemberInfo(property, out _, out var listMemberInfo);
            BetterListAttribute attribute = listMemberInfo.GetCustomAttribute<BetterListAttribute>();
            if (attribute != null)
                ApplyAttribute(attribute);
        }

        private void ApplyAttribute(BetterListAttribute attribute) {
            AlwaysExpanded = attribute.AlwaysExpanded;

            InnerList.displayRemove = attribute.ShowRemove;
            ShowFocus = attribute.ShowRemove;
            InnerList.displayAdd = attribute.ShowAdd;

            if (attribute.AddMethod != null) {
                SetAddMethod(attribute.AddMethod);
            }
            if (attribute.RemoveMethod != null) {
                SetRemoveMethod(attribute.RemoveMethod);
            }
        }

        private void SetAddMethod(string attributeAddMethod) {
            try {
                EditorUtils.GetMemberInfo(InnerList.serializedProperty, attributeAddMethod, out var targetObject,
                    out var memberInfo);
                MethodInfo methodInfo = (MethodInfo) memberInfo;

                if (methodInfo.GetParameters().Length != 0)
                    throw new ArgumentException();

                InnerList.onAddCallback = list => {
                    methodInfo.Invoke(targetObject, new object[0]);
                };
            }
            catch (Exception) {
                AddMethodError = true;
            }
        }
        
        
        private void SetRemoveMethod(string attributeRemoveMethod) {
            try {
                EditorUtils.GetMemberInfo(InnerList.serializedProperty, attributeRemoveMethod, out var targetObject,
                    out var memberInfo);
                MethodInfo methodInfo = (MethodInfo) memberInfo;

                var parameters = methodInfo.GetParameters();
                if (parameters.Length != 1 || 
                    parameters[0].ParameterType != typeof(int) && !EditorUtils.CompareType(parameters[0].ParameterType, InnerList.serializedProperty.arrayElementType))
                    throw new ArgumentException();

                InnerList.onRemoveCallback = list => {
                    if (parameters[0].ParameterType == typeof(int)) {
                        methodInfo.Invoke(targetObject, new object[] { list.index });
                    }
                    else {
                        EditorUtils.GetMemberInfo(list.serializedProperty, out var listParentObject, out var listMember);
                        IList valueList = EditorUtils.GetGeneralValue<IList>(listParentObject, listMember);
                        methodInfo.Invoke(targetObject, new[] { valueList[list.index] });
                    }
                };
            }
            catch (Exception e) {
                Debug.Log(e.GetType().Name);
                RemoveMethodError = true;
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isactive, bool isfocused) {
            rect.width -= 2;
            EditorGUI.DrawRect(rect, (index % 2 == 1) ? EditorUtils.BackgroundColor : EditorUtils.AccentBackgroundColor);
            if (isfocused && ShowFocus) {
                rect.width = 1.5f;
                EditorGUI.DrawRect(rect, Color.blue);
            }
        }

        public float GetHeight(SerializedProperty property, GUIContent label) {
            InnerList.serializedProperty = property;

            float height = 0;
            if (property.isExpanded || AlwaysExpanded)
                height += InnerList.GetHeight();
            else
                height += /* InnerList.HeaderHeight */ 20;

            if (AddMethodError) height += EditorUtils.HelpBox.GetHeight(AddMethodErrorText, HelpBoxType.Error);
            if (RemoveMethodError) height += EditorUtils.HelpBox.GetHeight(RemoveMethodErrorText, HelpBoxType.Error);
            
            return height;
        }

        public Rect OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            InnerList.serializedProperty = property;
            Rect listRect = new Rect(position) {height = GetHeight(property, label)};
            
            if (AddMethodError) listRect = EditorUtils.HelpBox.Draw(listRect, AddMethodErrorText, HelpBoxType.Error);
            if (RemoveMethodError) listRect = EditorUtils.HelpBox.Draw(listRect, RemoveMethodErrorText, HelpBoxType.Error);
            
            if (property.isExpanded || AlwaysExpanded)
                InnerList.DoList(listRect);
            else
                DrawListHeaderOnly(listRect);

            position.ToBottomOf(listRect);
            return position;
        }

        private void DrawListHeaderOnly(Rect rect) {
            // Copied from decompiler
            ReorderableList.defaultBehaviours.DrawHeaderBackground(rect);
            rect.xMin += 6f;
            rect.xMax -= 6f;
            rect.height -= 2f;
            ++rect.y;

            DrawListHeader(rect);
        }

        private void DrawListHeader(Rect headerRect) {
            const float indentSize = 15;
            const float arrowWidth = 12;
            float displacement = EditorGUI.indentLevel * indentSize - (AlwaysExpanded ? 0 : arrowWidth);
            
            headerRect.x -= displacement;
            headerRect.width += displacement;
            if (! AlwaysExpanded)
                InnerList.serializedProperty.isExpanded = EditorGUI.Foldout(headerRect, InnerList.serializedProperty.isExpanded, CurrentProperty.displayName, true);
            else 
                EditorGUI.LabelField(headerRect, CurrentProperty.displayName);
        }
        
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            SerializedProperty property = GetPropertyAt(index);
            FieldDrawer.Draw(property.Copy(), rect, false);
        }
        
        private float GetElementHeight(int index) {
            return FieldDrawer.GetPropertyHeight(GetPropertyAt(index).Copy(), false);
        }

        private SerializedProperty GetPropertyAt(int index) {
            SerializedProperty property = CurrentProperty.GetArrayElementAtIndex(index);
            return property;
        }
    }
}
