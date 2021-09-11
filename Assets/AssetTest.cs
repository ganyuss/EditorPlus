using EditorPlus;
using UnityEngine;

public class AssetTest : ScriptableObject {
    public int test;
    [CustomSpace(20)]
    [Disabled]
    public int test2;

    public MinMaxInt minMax1;
    [HelpBox("This field is convenient", HelpBoxType.Info)]
    [CustomSpace(20, 10)]
    [MinMaxSlider(0, 20)]
    public MinMaxFloat minMax2;
    [MinMaxSlider(0, 20)]
    public float minMax0;

    [CustomSpace(20, 10)]
    [TextArea(2, 3)]
    public string testTextArea;
    [TextArea(2, 3)]
    public string comparison;
    [CustomSpace(20)]
    [Multiline(5)]
    public string testMultiline;
    [Multiline(5)]
    public string comparison2;
    
    [Button("test")]
    public void a() {
        Debug.Log("AAAAAAAAAAAAAAA");
    }
    
    [Button(Size = ButtonSize.Small)]
    public void c() {
        Debug.Log("CCCCCCCCCCCCCCCCCCC");
    }
    
    
    [Button(Size = ButtonSize.Regular)]
    public void d() {
        Debug.Log("DDDDDDDDDDDDDDDDDDDDDDDDD");
    }
    
    
    [Button(Size = ButtonSize.Large)]
    public void b() {
        Debug.Log("BBBBBBBBBBBBBBB");
    }
    
    [Button(Size = ButtonSize.ExtraLarge)]
    public void e() {
        Debug.Log("EEEEEEEEEEEEEEEEEEEEEE");
    }
}
