using System;
using UnityEngine;


namespace EditorPlus {
    
    [AttributeUsage(DecoratorAttribute.Targets)]
    public class MinMaxSliderAttribute : PropertyAttribute {

        public float SliderMin;
        public float SliderMax;

        public MinMaxSliderAttribute(float sliderMin, float sliderMax) {
            SliderMin = sliderMin;
            SliderMax = sliderMax;
        }
    }
}