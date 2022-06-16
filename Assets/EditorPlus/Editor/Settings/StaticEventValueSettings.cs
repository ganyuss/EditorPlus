using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor
{
    [FilePath("StaticEventValueSettings.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class StaticEventValueSettings : ScriptableSingleton<StaticEventValueSettings>
    {
        [SerializeField]
        internal List<string> _eventIds = new List<string>();

        public List<string> EventIds => _eventIds;
        
        void OnEnable()
        {
            hideFlags &= ~HideFlags.NotEditable;
        }
    }
}
