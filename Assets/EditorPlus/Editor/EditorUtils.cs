using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public static class EditorUtils {

        public const string MultipleValueString = "—";
        
        public static Color BackgroundColor {
            get {
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.22f, 0.22f, 0.22f);
                else {
                    return new Color(0.73f, 0.73f, 0.73f);
                }
            }
        }
        
        public static Color AccentBackgroundColor {
            get {
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.25f, 0.25f, 0.25f);
                else {
                    return new Color(0.78f, 0.78f, 0.78f);
                }
            }
        }

        public static void ToBottomOf(this ref Rect rect, Rect otherRect) {
            rect.y += otherRect.height;
            rect.height -= otherRect.height;
        }


        public static void GetMemberInfo(SerializedProperty property, out object targetObject,
            out MemberInfo targetMember) 
            => GetMemberInfo(property.serializedObject.targetObject, property, out targetObject, out targetMember);

        public static void GetMemberInfo(object parentObject, SerializedProperty property, out object targetObject, out MemberInfo targetMember) {
            if (string.IsNullOrEmpty(property.propertyPath)) {
                targetObject = null;
                targetMember = null;
                return;
            }

            List<string> memberPath = property.propertyPath.Split('.').ToList();

            GetMemberInfo(parentObject, memberPath, out targetObject, out targetMember);
        }
        
        public static void GetMemberInfo(object parentObject, SerializedProperty property, string relativeMemberPath, out object targetObject, out MemberInfo targetMember) {
            if (string.IsNullOrEmpty(property.propertyPath)) {
                targetObject = null;
                targetMember = null;
                return;
            }

            List<string> memberPath = property.propertyPath.Split('.').ToList();
            memberPath[memberPath.Count - 1] = relativeMemberPath;

            GetMemberInfo(parentObject, memberPath, out targetObject, out targetMember);
        }
        
        public static void GetMemberInfo(
                SerializedProperty property, string relativeMemberPath, 
                out object targetObject, out MemberInfo targetMember) {
            object startingObject = property.serializedObject.targetObject;
            
            List<string> memberPath = property.propertyPath.Split('.').ToList();
            memberPath[memberPath.Count - 1] = relativeMemberPath;

            GetMemberInfo(startingObject, memberPath, out targetObject, out targetMember);
        }

        public static void GetMemberInfo(object startingObject, List<string> memberPath,
                out object targetObject, out MemberInfo targetMember) {

            BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                                        | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.InvokeMethod;

            // Because we will change it, we copy it to prevent errors in the calling context
            memberPath = memberPath.ToList();
            targetMember = null;
            targetObject = startingObject;
            object nextObject = null;
            
            while (memberPath.Count > 0) {
                if (IsArrayPath(memberPath)) {

                    // If the path refers to an array element, we 
                    // return the array member
                    if (memberPath.Count == 2) {
                        memberPath.Clear();
                        continue;
                    }
                    
                    IList list = (IList) (nextObject ?? targetObject);
                    int valueIndex = GetArrayIndex(memberPath);

                    if (nextObject != null) {
                        targetObject = nextObject;
                    }
                    nextObject = list[valueIndex];
                    // In a property path, the path to the Nth element of an array is Array.data[N]
                    memberPath.RemoveAt(0);
                    memberPath.RemoveAt(0);
                }
                else {
                    targetMember = (nextObject ?? targetObject).GetType().GetMember(memberPath[0], BindingFlags).First();
                    memberPath.RemoveAt(0);

                    if (nextObject != null) {
                        targetObject = nextObject;
                    }
                    
                    if (memberPath.Count > 0) {
                        if (targetMember.MemberType == MemberTypes.Field) {
                            nextObject = ((FieldInfo) targetMember).GetValue(targetObject);
                        }
                        else {
                            throw new ArgumentException(
                                $"member {targetMember.Name} of class {targetObject.GetType().FullName} is " +
                                $"not a field, property or method");
                        }
                    }
                }
            }
        }
        
        public static T GetGeneralValue<T>(object obj, MemberInfo member) {
            if (member.MemberType == MemberTypes.Field) {
                return (T) ((FieldInfo) member).GetValue(obj);
            }
            if (member.MemberType == MemberTypes.Property) {
                return (T) ((PropertyInfo) member).GetValue(obj);
            }
            if (member.MemberType == MemberTypes.Method) {
                return (T) ((MethodInfo) member).Invoke(obj, new object[0]);
            }

            throw new ArgumentException("trying to get generic value of member that is not a property, field or method.");
        }
        
        public static void SetGeneralValue(object parentObject, MemberInfo member, object targetValue) {
            if (member.MemberType == MemberTypes.Field) {
                ((FieldInfo) member).SetValue(parentObject, targetValue);
                return;
            }
            if (member.MemberType == MemberTypes.Property) {
                ((PropertyInfo) member).SetValue(parentObject, targetValue);
                return;
            }

            throw new ArgumentException("trying to set generic value of member that is not a property or field.");
        }


        // In a property path, the path to the Nth element of an array is Array.data[N]
        private static readonly Regex ArrayDataRegex = new Regex(@"data\[([0-9]+)\]");
        
        private static bool IsArrayPath(List<string> memberPath) {
            return memberPath.Count >= 2 && memberPath[0] == "Array" && ArrayDataRegex.IsMatch(memberPath[1]);
        }
        
        private static int GetArrayIndex(List<string> memberPath) {
            // We get the first capture
            return int.Parse(ArrayDataRegex.Match(memberPath[1]).Groups[1].Captures[0].Value);
        }

        /// <summary>
        /// This method allows to compare a type to its serialized name.
        /// </summary>
        /// <param name="type">The type to test</param>
        /// <param name="serializedTypeName">The name as it appears in the serialized property</param>
        /// <returns>true if the type name describes the given type, false otherwise</returns>
        public static bool CompareType(Type type, string serializedTypeName) {
            Dictionary<string, Type> specialTypes = new Dictionary<string, Type> {
                {"int", typeof(Int32)},
                {"float", typeof(Single)},
                {"string", typeof(String)},
            };

            if (specialTypes.TryGetValue(serializedTypeName, out var specialType)) {
                return specialType == type;
            }

            return type.Name == serializedTypeName;
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
                
                return height + EditorGUIUtility.standardVerticalSpacing*2;
            }
            
            public static Rect GetRect(Rect position, string boxText, HelpBoxType type) {
                return new Rect(position) {height = GetHeight(boxText, type)};
            }

            public static Rect Draw(Rect position, string boxText, HelpBoxType type) {
                Rect helpBoxRect = GetRect(position, boxText, type);
                position.ToBottomOf(helpBoxRect);

                helpBoxRect.y += EditorGUIUtility.standardVerticalSpacing;
                helpBoxRect.height -= EditorGUIUtility.standardVerticalSpacing * 2;
                EditorGUI.HelpBox(helpBoxRect, boxText, GetUnityMessageType(type));
                
                
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
