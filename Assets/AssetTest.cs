using System;
using System.Collections.Generic;
using EditorPlus;
using UnityEngine;
using UnityEngine.Events;
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

    public StaticUnityEventInvoker Invoker;

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
    
    [HelpBox("List propagation test")]
    [BetterList(AlwaysExpanded = true)]
    public string[] ListDrawer;

    public E EEEEEEEEEE;

    public string ZZZZzz;
    public string ZZZZzzz;
    public string ZZZZzzzz;

    [Dropdown(new[] { "test1", "test2" })]
    public string DropdownTest;
    
    [Dropdown(nameof(dropdownInt))]
    public int DropdownTest2;

    private DropdownList<int> dropdownInt() => new DropdownList<int> {["one"] = 1, ["two"] = 2, ["three"] = 3};

    [Tag]
    public string TagExample;
    
    [Dropdown(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
    public int Digit;

    [OnValueChanged(nameof(PrintMonitoredValue))]
    public int ThisValueIsMonitored;

    private void PrintMonitoredValue() {
        Debug.Log(ThisValueIsMonitored);
    }
    
    public UnityEvent CustomEditorsTest;

    //[CustomSpace(10 ,10)]
    [Button("test")]
    public void a() {
        Debug.Log("1: " + DropdownTest);
        Debug.Log("2: " + DropdownTest2);
    }
    
    [DisableIf(nameof(ZZZZzz), "slt")]
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
        public V[] W;
        [HideIf(nameof(zzzz))]
        public string str;
        [ShowIf(nameof(zzzz))]
        public string str2;
        [Indent(2)]
        public bool zzzz;

        [Button("WWWWWWWWWWWWWWWWWWWW")]
        [ShowIf(nameof(zzzz))]
        private void test() {
            Debug.Log("AAAAAAAAAAAAAAA z");
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
