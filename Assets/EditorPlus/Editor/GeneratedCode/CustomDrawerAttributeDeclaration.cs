using UnityEditor;

namespace EditorPlus.Editor {

	[CustomPropertyDrawer(typeof(EditorPlus.DisableIfAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.DisabledAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.HideInEditModeAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.DisableInEditModeAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.HelpBoxAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.HideIfAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.HideInPlayModeAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.DisableInPlayModeAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.CustomSpaceAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.Editor.DefaultPropertyAttribute))]
	[CustomPropertyDrawer(typeof(EditorPlus.MinMaxSliderAttribute))]
	[CustomPropertyDrawer(typeof(UnityEngine.MultilineAttribute))]
	[CustomPropertyDrawer(typeof(UnityEngine.RangeAttribute))]
	[CustomPropertyDrawer(typeof(UnityEngine.TextAreaAttribute))]
	public partial class CustomUnityDrawer { }
}
