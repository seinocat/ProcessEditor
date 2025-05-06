using System;
using UnityEngine;

namespace Seino.Utils
{
    public static partial class SeinoUtils
    {
        #region 单值插值

        /// <summary>
        /// 线性插值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="increaseSpeed"></param>
        /// <param name="decreaseSpeed"></param>
        /// <returns></returns>
        public static float LerpValue(float value, float target, float increaseSpeed, float decreaseSpeed) {
            if (Math.Abs(value - target) < 0.01f) return target; 
            if (value < target) return Mathf.Clamp(value + Time.deltaTime * increaseSpeed, -Mathf.Infinity, target);
            return Mathf.Clamp(value - Time.deltaTime * decreaseSpeed, target, Mathf.Infinity);
        }
        
        /// <summary>
        /// 平滑插值 shader算法
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float SmoothStep(float t1, float t2, float x)
        {
            x = Mathf.Clamp((x - t1) / (t2 - t1), 0.0f, 1.0f);
            return x * x * (3 - 2 * x);
        }
        
        /// <summary>
        /// 非线性插值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="velocity"></param>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static float LerpDamper(float value, float target, ref float velocity, float time, LerpType type)
        {
            switch (type)
            {
                case LerpType.SampleDamper:
                    return SampleDamper(value, target, ref velocity, time);
                case LerpType.Damper:
                    return Mathf.SmoothDamp(value, target, ref velocity, time);
            }

            return 0;
        }
        
        /// <summary>
        /// 简化临界阻尼插值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="velocity"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float SampleDamper(float value, float target, ref float velocity, float time)
        {
            var omega = 2.0f / time;
            float x = omega * Time.deltaTime;
            float exp = 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);
            float change = value - target;
            float temp = (velocity + omega * change) * Time.deltaTime;
            velocity = (velocity - omega * temp) * exp;
            return target + (change + temp) * exp;
        }

        #endregion

        #region 多值插值

        public static Vector2 Lerp(this Vector2 value, Vector2 from, Vector2 to, float t, EaseType type = EaseType.Linear)
        {
            return Vector2.Lerp(from, to, Easing.Function(type)(t));
        }
        
        public static Vector3 Lerp(this Vector3 value, Vector3 from, Vector3 to, float t, EaseType type = EaseType.Linear)
        {
            return Vector3.Lerp(from, to, Easing.Function(type)(t));
        }
        
        public static Vector4 Lerp(this Vector4 value, Vector4 from, Vector4 to, float t, EaseType type = EaseType.Linear)
        {
            return Vector4.Lerp(from, to, Easing.Function(type)(t));
        }
        
        public static Color Lerp(this Color value, Color from, Color to, float t, EaseType type = EaseType.Linear)
        {
            return Color.Lerp(from, to, Easing.Function(type)(t));
        }
        
        public static Quaternion Lerp(this Quaternion value, Quaternion from, Quaternion to, float t, EaseType type = EaseType.Linear)
        {
            return Quaternion.Lerp(from, to, Easing.Function(type)(t));
        }
        
        public static Quaternion Slerp(this Quaternion value, Quaternion from, Quaternion to, float t, EaseType type = EaseType.Linear)
        {
            return Quaternion.Slerp(from, to, Easing.Function(type)(t));
        }

        #endregion
    }
}