using System;
using UnityEngine;


namespace EditorPlus {
    /// <summary>
    /// Can be used with <see cref="MinMaxSliderAttribute"/>
    /// </summary>
    [Serializable]
    public class MinMaxInt {
        public int Min;
        public int Max;

        /// <summary>
        /// Calls <see cref="UnityEngine.Random.Range(int, int)"/> with <see cref="Min"/> and <see cref="Max"/> as parameters.
        /// </summary>
        /// <returns>a random int within [Min..Max) (Max is exclusive)</returns>
        public int Random()
        {
            return UnityEngine.Random.Range(Min, Max);
        }

        /// <summary>
        /// Calls <see cref="Mathf.Lerp(float, float, float)"/> with with <see cref="Min"/> and <see cref="Max"/>
        /// as the 2 first parameters, interpolating a value between Min and Max by t.
        /// </summary>
        /// <param name="t">The interpolation value between Min and Max.</param>
        /// <returns>The interpolated float result between Min and Max.</returns>
        public float Lerp(float t)
        {
            return Mathf.Lerp(Min, Max, t);
        }

        /// <summary>
        /// Calls <see cref="Mathf.Clamp(int, int, int)"/>with with <see cref="Min"/> and <see cref="Max"/>
        /// as the 2 last parameters, clamping the given value between Min and Max.
        /// </summary>
        /// <param name="value">The integer point value to restrict inside the Min-to-Max range</param>
        /// <returns>The int result between Min and Max values.</returns>
        public int Clamp(int value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
    }

    /// <summary>
    /// Can be used with <see cref="MinMaxSliderAttribute"/>
    /// </summary>
    [Serializable]
    public class MinMaxFloat {
        public float Min;
        public float Max;

        /// <summary>
        /// Calls <see cref="UnityEngine.Random.Range(float, float)"/> with <see cref="Min"/> and <see cref="Max"/>
        /// </summary>
        /// <returns>a random int within [Min..Max) (Max is exclusive)</returns>
        public float Random()
        {
            return UnityEngine.Random.Range(Min, Max);
        }
        
        /// <summary>
        /// Calls <see cref="Mathf.Lerp(float, float, float)"/> with with <see cref="Min"/> and <see cref="Max"/>
        /// as the 2 first parameters, interpolating a value between Min and Max by t.
        /// </summary>
        /// <param name="t">The interpolation value between Min and Max.</param>
        /// <returns>The interpolated float result between Min and Max.</returns>
        public float Lerp(float t)
        {
            return Mathf.Lerp(Min, Max, t);
        }

        /// <summary>
        /// Calls <see cref="Mathf.Clamp(float, float, float)"/>with with <see cref="Min"/> and <see cref="Max"/>
        /// as the 2 last parameters, clamping the given value between Min and Max.
        /// </summary>
        /// <param name="value">The integer point value to restrict inside the Min-to-Max range</param>
        /// <returns>The int result between Min and Max values.</returns>

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
    }
}
