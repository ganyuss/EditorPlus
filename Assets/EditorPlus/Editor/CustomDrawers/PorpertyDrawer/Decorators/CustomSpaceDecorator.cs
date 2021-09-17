using System.Collections.Generic;
using EditorPlus;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class CustomSpaceDecorator : DecoratorBase<CustomSpaceAttribute> {

        public override OrderValue Order => OrderValue.VeryFirst;

        public override float GetHeight(List<object> targets, string memberPath, SerializedProperty property) {
            return CurrentAttribute.SpaceBefore + CurrentAttribute.SpaceAfter;
        }

        public override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            position.y += CurrentAttribute.SpaceBefore;
            position.height -= CurrentAttribute.SpaceBefore;
            return position;
        }
        
        public override Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            position.y += CurrentAttribute.SpaceAfter;
            position.height -= CurrentAttribute.SpaceAfter;
            return position;
        }
    }
}
