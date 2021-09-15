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
        public V[] W;
        public string str;
        public string str2;
    }

    [Serializable]
    public class V {
        public string VSTR;
        public int wow;
    }
}
