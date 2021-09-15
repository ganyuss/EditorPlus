using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class HideInPlayModeDecorator : DecoratorBase<HideInPlayModeAttribute> {
        public override bool ShowProperty(SerializedProperty property) => !EditorApplication.isPlaying;
    }
    
    public class DisableInPlayModeDecorator : DisableIfDecoratorBase<DisableInPlayModeAttribute> {
        protected override bool Disable(SerializedProperty property) {
            return EditorApplication.isPlaying;
        }
    }
}

