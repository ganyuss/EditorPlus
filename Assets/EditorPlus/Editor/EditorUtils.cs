using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public static class EditorUtils {

        public const string MultipleValueString = "—";

        public static void ToBottomOf(this ref Rect rect, Rect otherRect) {
            rect.y += otherRect.height;
            rect.height -= otherRect.height;
        }

        public static Vector2 ScrollView(Rect scrollViewPosition, float innerHeight, Vector2 scroll, Action<Rect> insideScrollView) {
            Rect innerRect = new Rect(0, 0, scrollViewPosition.width, innerHeight);
            if (innerHeight > scrollViewPosition.height) {
                innerRect.width -= GUI.skin.verticalScrollbar.fixedWidth;
            }

            Vector2 newScroll = GUI.BeginScrollView(scrollViewPosition, scroll, innerRect);
            insideScrollView.Invoke(innerRect);
            GUI.EndScrollView();

            return newScroll;
        }

        public static FieldInfo GetFieldInfo(this SerializedProperty property) {
            return property.serializedObject.targetObject.GetType().GetField(property.propertyPath);
        }


        public static class HelpBox {
            private const int paddingHeight = 8;
            private const int marginHeight = 2;

            public static float GetHeight(string boxText, HelpBoxType type) {
                // This stops icon shrinking if text content doesn't fill out the container enough.
                float minHeight = paddingHeight * 5;

                // Calculate the height of the HelpBox using the GUIStyle on the current skin and the inspector
                // window's currentViewWidth.
                var content = new GUIContent(boxText);
                if (type != HelpBoxType.None)
                    content.image = new Texture2D(55, 0);
                var style = GUI.skin.GetStyle("helpbox");

                var height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);

                // We add tiny padding here to make sure the text is not overflowing the HelpBox from the top
                // and bottom.
                height += marginHeight * 2;

                // If the calculated HelpBox is less than our minimum height we use this to calculate the returned
                // height instead.
                if (type != HelpBoxType.None && height < minHeight)
                    height = minHeight;
                
                return height;
            }
            
            public static Rect GetRect(Rect position, string boxText, HelpBoxType type) {
                return new Rect(position) {height = GetHeight(boxText, type)};
            }

            public static Rect Draw(Rect position, string boxText, HelpBoxType type) {
                Rect helpBoxRect = GetRect(position, boxText, type);
                EditorGUI.HelpBox(helpBoxRect, boxText, GetUnityMessageType(type));
                
                position.ToBottomOf(helpBoxRect);
                
                return position;
            }

            private static MessageType GetUnityMessageType(HelpBoxType helpBoxType) {
                switch (helpBoxType) {
                    case HelpBoxType.None: return MessageType.None;
                    case HelpBoxType.Info: return MessageType.Info;
                    case HelpBoxType.Warning: return MessageType.Warning;
                    case HelpBoxType.Error: return MessageType.Error;
                }
                
                return MessageType.None;
            }
        }
    }
}
