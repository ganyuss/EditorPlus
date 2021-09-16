using System;
using EditorPlus;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetTest : ScriptableObject {
    [CustomSpace(20)]
    [Disabled]
    public int Test;

    [HelpBox("This field is convenient", HelpBoxType.Info)]
    [MinMaxSlider(0, 20)]
    public MinMaxInt MinMax;

    [TextArea(2, 3)]
    public string TextArea;
    [Multiline(5)]
    public string Multiline;
    [Multiline(5)]
    public string Comparison;
    [Range(0, 100)]
    public float Range;

    [HideInPlayMode]
    [SerializeField]
    private int PlayModeOnly;
    
    [DisableIf(nameof(Disabled))]
    public string field;
    public bool Disabled;
    
    [BetterList(AlwaysExpanded = false)]
    public string[] ListDrawer;

    public E EEEEEEEEEE;
    
    [CustomSpace(10 ,10)]
    [Button("test")]
    public void a() {
        Debug.Log("AAAAAAAAAAAAAAA");
    }
    
    [Button(Size = ButtonSize.ExtraLarge)]
    public void e() {
        Debug.Log("EEEEEEEEEEEEEEEEEEEEEE");
    }

#if UNITY_EDITOR
    [OnEditorGUI]
    private void OnEditorGUI() {
        EditorGUILayout.LabelField("OnEditorGUI did this");
    }
#endif

    [Serializable]
    public class E {
        [BetterList(AddMethod = nameof(Add), RemoveMethod = nameof(Remove))]
        public V[] W;
        [HideIf(nameof(zzzz))]
        public string str;
        [ShowIf(nameof(zzzz))]
        public string str2;
        public bool zzzz;

        private void Add() {
            Debug.Log("ass");
        }

        private void Remove(V v) {
            Debug.Log("remove element " + v.VSTR);
        }
    }

    [Serializable]
    public class V {
        [EnableIf(nameof(disabled))]
        public string VSTR;
        [DisableIf(nameof(disabled))]
        public int wow;
        public bool disabled;
        public int[] values;
    }
}
