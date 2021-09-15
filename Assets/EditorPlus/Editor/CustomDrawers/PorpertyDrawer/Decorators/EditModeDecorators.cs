using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class HideInEditModeDecorator : DecoratorBase<HideInEditModeAttribute> {
        public override bool ShowProperty(SerializedProperty property) => EditorApplication.isPlaying;
    }
    
    public class DisableInEditModeDecorator : DisableIfDecoratorBase<DisableInEditModeAttribute> {
        protected override bool Disable(SerializedProperty property) {
            return !EditorApplication.isPlaying;
        }
    }
}

