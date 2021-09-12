using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace EditorPlus.Editor {
    public class MinMaxDrawer : PropertySpecificDrawerBase<MinMaxSliderAttribute> {
        private const float rightFieldSize = 50f;
        private const float rightMargin = 5f;
        
        private SerializedProperty GetMinProperty(SerializedProperty property) {
            switch (property.type) {
                case nameof(MinMaxInt):
                    return property.FindPropertyRelative(nameof(MinMaxInt.Min));
                case nameof(MinMaxFloat):
                    return property.FindPropertyRelative(nameof(MinMaxFloat.Min));
                default:
                    return null;
            }
        }

        private SerializedProperty GetMaxProperty(SerializedProperty property) {
            switch (property.type) {
                case nameof(MinMaxInt):
                    return property.FindPropertyRelative(nameof(MinMaxInt.Max));
                case nameof(MinMaxFloat):
                    return property.FindPropertyRelative(nameof(MinMaxFloat.Max));
                default:
                    return null;
            }
        }

        protected override float GetRealHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUI.BeginChangeCheck();
            
            SerializedProperty minProperty = GetMinProperty(property);
            SerializedProperty maxProperty = GetMaxProperty(property);

            float minValue;
            float maxValue;
            
            if (property.type == nameof(MinMaxInt)) {
                minValue = minProperty.intValue;
                maxValue = maxProperty.intValue;
            }
            else {
                minValue = minProperty.floatValue;
                maxValue = maxProperty.floatValue;
            }
            
            float oldMinValue = minValue;
            float oldMaxValue = maxValue;

            MinMaxSliderAttribute minMaxAttribute = CurrentAttribute;

            Rect sliderPosition = new Rect(position);
            sliderPosition.width -= rightFieldSize*2 + rightMargin*2;
            sliderPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.MinMaxSlider(sliderPosition, label, ref minValue, ref maxValue, minMaxAttribute.SliderMin, minMaxAttribute.SliderMax);

            Rect firstFieldPosition = new Rect(sliderPosition);
            firstFieldPosition.xMin += sliderPosition.width + rightMargin;
            firstFieldPosition.width = rightFieldSize;
            
            Rect secondFieldPosition = new Rect(firstFieldPosition);
            secondFieldPosition.xMin += rightFieldSize + rightMargin;
            secondFieldPosition.width = rightFieldSize;
            
            if (property.type == nameof(MinMaxInt)) {
                minValue = EditorGUI.IntField(firstFieldPosition, Mathf.RoundToInt(minValue));
                maxValue = EditorGUI.IntField(secondFieldPosition, Mathf.RoundToInt(maxValue));
            }
            else {
                minValue = EditorGUI.FloatField(firstFieldPosition, minValue);
                maxValue = EditorGUI.FloatField(secondFieldPosition, maxValue);
            }

            if (EditorGUI.EndChangeCheck()) {

                minValue = Mathf.Clamp(minValue, minMaxAttribute.SliderMin, minMaxAttribute.SliderMax);
                maxValue = Mathf.Clamp(maxValue, minMaxAttribute.SliderMin, minMaxAttribute.SliderMax);
                
                if (minValue > maxValue) {
                    if (oldMinValue != minValue)
                        minValue = maxValue;
                    else if (oldMaxValue != maxValue)
                        maxValue = minValue;
                }

                if (property.type == nameof(MinMaxInt)) {
                    minProperty.intValue = Mathf.RoundToInt(minValue);
                    maxProperty.intValue = Mathf.RoundToInt(maxValue);
                }
                else {
                    minProperty.floatValue = minValue;
                    maxProperty.floatValue = maxValue;
                }
            }

            position.ToBottomOf(sliderPosition);
            return position;
        }

        protected override bool IsPropertyValid(SerializedProperty property, GUIContent label) {
            return property.type == nameof(MinMaxInt) || property.type == nameof(MinMaxFloat);
        }

        protected override string GetErrorText(SerializedProperty property, GUIContent label) {
            return $"The {nameof(MinMaxSlider)} is set on the {property.name} property of type {property.type}. " +
                $"{nameof(MinMaxSlider)} only works on {nameof(MinMaxInt)} or {nameof(MinMaxFloat)} properties.";
        }
    }
}
