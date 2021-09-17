using System.Collections.Generic;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;


namespace EditorPlus.Editor {
    public class SpaceDecorator : DecoratorBase<SpaceAttribute> {
        public override OrderValue Order => OrderValue.First;

        public override float GetHeight(List<object> targets, string memberPath, SerializedProperty property) {            // We do not want spaces to propagate inside lists
            if (EditorUtils.IsForArrayElement(memberPath))
                return 0;
            return CurrentAttribute.height;
        }

        public override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath,
            SerializedProperty property) {
            if (EditorUtils.IsForArrayElement(memberPath))
                return position;
            
            position.y += CurrentAttribute.height;
            position.height -= CurrentAttribute.height;
            return position;
        }
    }
}
