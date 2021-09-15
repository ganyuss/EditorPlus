using System;
using EditorPlus.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public class ListPropertyDrawer {
        
        private ReorderableList InnerList;
        private SerializedProperty CurrentProperty;
        private SerializedFieldDrawer FieldDrawer = new SerializedFieldDrawer();

        public ListPropertyDrawer(SerializedProperty property) {
            CurrentProperty = property.Copy();
            InnerList = new ReorderableList(CurrentProperty.serializedObject, CurrentProperty) {
                drawHeaderCallback = DrawListHeader,
                drawElementCallback = DrawElement,
                elementHeightCallback = GetElementHeight,
                drawElementBackgroundCallback = DrawElementBackground,
            };
        }

        private void DrawElementBackground(Rect rect, int index, bool isactive, bool isfocused) {
            rect.width -= 2;
            EditorGUI.DrawRect(rect, (index % 2 == 1) ? EditorUtils.BackgroundColor : EditorUtils.AccentBackgroundColor);
        }

        public float GetHeight(SerializedProperty property, GUIContent label) {
            InnerList.serializedProperty = property;
            return InnerList.GetHeight();
        }

        public Rect OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            InnerList.serializedProperty = property;
            Rect listRect = new Rect(position) {height = GetHeight(property, label)};
            InnerList.DoList(listRect);

            position.ToBottomOf(listRect);
            return position;
        }

        private void DrawListHeader(Rect headerRect) {
            headerRect.x -= EditorGUI.indentLevel * 15;
            headerRect.width += EditorGUI.indentLevel * 15;
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
