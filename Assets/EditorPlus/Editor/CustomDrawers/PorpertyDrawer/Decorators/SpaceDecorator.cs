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

        public override Rect OnBeforeGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.y += CurrentAttribute.SpaceBefore;
            position.height -= CurrentAttribute.SpaceBefore;
            return position;
        }
        
        public override Rect OnAfterGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.y += CurrentAttribute.SpaceAfter;
            position.height -= CurrentAttribute.SpaceAfter;
            return position;
        }
    }
}
