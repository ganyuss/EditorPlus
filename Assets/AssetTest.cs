using System;
using EditorPlus;
using UnityEngine;

public class AssetTest : ScriptableObject {
    [CustomSpace(20)]
    [Disabled]
    public int Test;

    [HelpBox("This field is convenient", HelpBoxType.Info)]
    [CustomSpace(20, 10)]
    [MinMaxSlider(0, 20)]
    public MinMaxFloat MinMax;

    [CustomSpace(10, 10)]
    [TextArea(2, 3)]
    public string TextArea;
    [CustomSpace(10 ,10)]
    [Multiline(5)]
    public string Multiline;
    [CustomSpace(10 ,10)]
    [Range(0, 100)]
    public float Range;
    
    [Button("test")]
    public void a() {
        Debug.Log("AAAAAAAAAAAAAAA");
    }

    [Button(Size = ButtonSize.ExtraLarge)]
    public void e() {
        Debug.Log("EEEEEEEEEEEEEEEEEEEEEE");
    }
}
