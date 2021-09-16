using EditorPlus;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class SpaceDecorator : DecoratorBase<CustomSpaceAttribute> {

        public override OrderValue Order => OrderValue.VeryFirst;

        public override float GetHeight(SerializedProperty property, GUIContent label) {
            return CurrentAttribute.SpaceBefore + CurrentAttribute.SpaceAfter;
        }

        protected override Rect OnBeforeGUI(Rect position, string memberPath, SerializedProperty property) {
            position.y += CurrentAttribute.SpaceBefore;
            position.height -= CurrentAttribute.SpaceBefore;
            return position;
        }
        
        protected override Rect OnAfterGUI(Rect position, string memberPath, SerializedProperty property) {
            position.y += CurrentAttribute.SpaceAfter;
            position.height -= CurrentAttribute.SpaceAfter;
            return position;
        }
    }
}
