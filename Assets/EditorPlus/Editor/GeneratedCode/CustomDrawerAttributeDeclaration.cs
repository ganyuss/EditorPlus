using UnityEditor;

namespace EditorPlus.Editor {

	[CustomPropertyDrawer(typeof(EditorPlus.DisabledAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.HelpBoxAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.CustomSpaceAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.Editor.DefaultPropertyAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.MinMaxSliderAttribute))]
	[CustomPropertyDrawer(typeof(UnityEngine.MultilineAttribute))]
	[CustomPropertyDrawer(typeof(UnityEngine.TextAreaAttribute))]
	public partial class CustomUnityDrawer { }
}
