using UnityEngine;

namespace EditorPlus {
    public class BetterListAttribute : PropertyAttribute {
        public bool AlwaysExpanded = false;
        
        public string AddMethod = null;
        public string RemoveMethod = null;

        public bool ShowAdd = true;
        public bool ShowRemove = true;
    }
}