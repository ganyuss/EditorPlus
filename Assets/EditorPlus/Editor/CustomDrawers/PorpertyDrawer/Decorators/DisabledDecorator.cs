using UnityEditor;

namespace EditorPlus.Editor {
    public class DisabledDecorator : DisableIfDecoratorBase<DisabledAttribute> {
        protected override bool Disable(SerializedProperty property) {
            return true;
        }
    }
}
