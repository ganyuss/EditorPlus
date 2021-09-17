using System;
using UnityEngine;


namespace EditorPlus {
    
    [AttributeUsage(EditorPlusAttribute.AttributeDrawerTargets)]
    public class MinMaxSliderAttribute : PropertyAttribute {

        public float SliderMin;
        public float SliderMax;

        public MinMaxSliderAttribute(float sliderMin, float sliderMax) {
            SliderMin = sliderMin;
            SliderMax = sliderMax;
        }
    }
}