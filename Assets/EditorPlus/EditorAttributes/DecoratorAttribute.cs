using System;
using UnityEngine;

namespace EditorPlus {
    public class EditorPlusAttribute {
        public const AttributeTargets DecoratorTargets = AttributeTargets.Field | AttributeTargets.Method;
        public const AttributeTargets AttributeDrawerTargets = AttributeTargets.Field;
    }
}
