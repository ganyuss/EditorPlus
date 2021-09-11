using UnityEngine;


namespace EditorPlus {
    public class MinMaxSliderAttribute : PropertyAttribute {

        public float SliderMin;
        public float SliderMax;

        public MinMaxSliderAttribute(float sliderMin, float sliderMax) {
            SliderMin = sliderMin;
            SliderMax = sliderMax;
        }
    }
}