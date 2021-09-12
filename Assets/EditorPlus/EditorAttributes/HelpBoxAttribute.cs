using System;
using UnityEngine;

namespace EditorPlus {
    
    public enum HelpBoxType {
        None,
        Info,
        Warning,
        Error
    }
    
    public enum HelpBoxPosition {
        Before,
        After
    }
    
    [AttributeUsage(
        AttributeTargets.Method 
        | AttributeTargets.Field
        | AttributeTargets.Property)]
    public class HelpBoxAttribute : PropertyAttribute {
        public HelpBoxType Type = HelpBoxType.None;
        public HelpBoxPosition Position = HelpBoxPosition.Before;
        public string Text;
        
        public HelpBoxAttribute(string text) => Text = text;
        
        public HelpBoxAttribute(string text, HelpBoxType type) : this(text) {
            Type = type;
        }
    }
}
