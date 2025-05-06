using System;
using UnityEngine;

namespace Seino.Utils
{
    
    ///<summary>Easing functions to be used for interpolation</summary>
    public static class Easing
    {

        ///----------------------------------------------------------------------------------------------

        public static float Ease(EaseType type, float from, float to, float t) {
            if ( t <= 0 ) return from;
            if ( t >= 1 ) return to;
            return Mathf.LerpUnclamped(from, to, Function(type)(t));
        }

        public static Vector3 Ease(EaseType type, Vector3 from, Vector3 to, float t) {
            if ( t <= 0 ) return from;
            if ( t >= 1 ) return to;
            return Vector3.LerpUnclamped(from, to, Function(type)(t));
        }

        public static Quaternion Ease(EaseType type, Quaternion from, Quaternion to, float t) {
            if ( t <= 0 ) return from;
            if ( t >= 1 ) return to;
            return Quaternion.LerpUnclamped(from, to, Function(type)(t));
        }

        public static Color Ease(EaseType type, Color from, Color to, float t) {
            if ( t <= 0 ) return from;
            if ( t >= 1 ) return to;
            return Color.LerpUnclamped(from, to, Function(type)(t));
        }

        ///----------------------------------------------------------------------------------------------

        public static Func<float, float> Function(EaseType type)
        {
            return type switch
            {
                EaseType.Linear => Linear,
                EaseType.QuadraticIn => QuadraticIn,
                EaseType.QuadraticOut => QuadraticOut,
                EaseType.QuadraticInOut => QuadraticInOut,
                EaseType.QuarticIn => QuarticIn,
                EaseType.QuarticOut => QuarticOut,
                EaseType.QuarticInOut => QuarticInOut,
                EaseType.QuinticIn => QuinticIn,
                EaseType.QuinticOut => QuinticOut,
                EaseType.QuinticInOut => QuinticInOut,
                EaseType.CubicIn => CubicIn,
                EaseType.CubicOut => CubicOut,
                EaseType.CubicInOut => CubicInOut,
                EaseType.ExponentialIn => ExponentialIn,
                EaseType.ExponentialOut => ExponentialOut,
                EaseType.ExponentialInOut => ExponentialInOut,
                EaseType.CircularIn => CircularIn,
                EaseType.CircularOut => CircularOut,
                EaseType.CircularInOut => CircularInOut,
                EaseType.SinusoidalIn => SinusoidalIn,
                EaseType.SinusoidalOut => SinusoidalOut,
                EaseType.SinusoidalInOut => SinusoidalInOut,
                EaseType.ElasticIn => ElasticIn,
                EaseType.ElasticOut => ElasticOut,
                EaseType.ElasticInOut => ElasticInOut,
                EaseType.BounceIn => BounceIn,
                EaseType.BounceOut => BounceOut,
                EaseType.BounceInOut => BounceInOut,
                EaseType.BackIn => BackIn,
                EaseType.BackOut => BackOut,
                EaseType.BackInOut => BackInOut,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        ///----------------------------------------------------------------------------------------------

        public static float Linear(float t) {
            return t;
        }

        public static float QuadraticIn(float t) {
            return t * t;
        }

        public static float QuadraticOut(float t) {
            return 1f - ( 1f - t ) * ( 1f - t );
        }

        public static float QuadraticInOut(float t) {
            return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }

        public static float QuarticIn(float t) {
            return t * t * t * t;
        }

        public static float QuarticOut(float t) {
            return 1f - (t * t * t * t );
        }

        public static float QuarticInOut(float t) {
            if ( ( t *= 2f ) < 1f )
                return 0.5f * t * t * t * t;
            return -0.5f * ( ( t -= 2f ) * t * t * t - 2f );
        }

        public static float QuinticIn(float t) {
            return t * t * t * t * t;
        }

        public static float QuinticOut(float t) {
            return t * t * t * t * t + 1f;
        }

        public static float QuinticInOut(float t) {
            if ( ( t *= 2f ) < 1 )
                return 0.5f * t * t * t * t * t;
            return 0.5f * ( ( t -= 2f ) * t * t * t * t + 2f );
        }

        public static float CubicIn(float t) {
            return t * t * t;
        }

        public static float CubicOut(float t) {
            return t * t * t + 1f;
        }

        public static float CubicInOut(float t) {
            return t < 0.5 ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }

        public static float SinusoidalIn(float t) {
            return 1f - Mathf.Cos(t * Mathf.PI / 2f);
        }

        public static float SinusoidalOut(float t) {
            return Mathf.Sin(t * Mathf.PI / 2f);
        }

        public static float SinusoidalInOut(float t) {
            return 0.5f * ( 1f - Mathf.Cos(Mathf.PI * t) );
        }

        public static float ExponentialIn(float t) {
            return t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
        }

        public static float ExponentialOut(float t) {
            return t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        }

        public static float ExponentialInOut(float t) {
            return t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) / 2f : ( 2f - Mathf.Pow(2f, -20f * t + 10f) ) / 2f;
        }

        public static float CircularIn(float t) {
            return 1f - Mathf.Sqrt(1f - t * t);
        }

        public static float CircularOut(float t) {
            return Mathf.Sqrt(1f - ( --t * t ));
        }

        public static float CircularInOut(float t) {
            return t < 0.5f ? ( Mathf.Sqrt(1f - t * t) - 1f ) / 2 : ( Mathf.Sqrt(1f - ( t -= 2f ) * t) + 1f ) / 2;
        }

        public static float ElasticIn(float t) {
            float x = ( 2f * Mathf.PI ) / 3f;
            return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin(( t * 10f - 10.75f ) * x);
        }

        public static float ElasticOut(float t) {
            float x = ( 2f * Mathf.PI ) / 3f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin(( t * 10f - 0.75f ) * x) + 1f;
        }

        public static float ElasticInOut(float t) {
            float x = ( 2f * Mathf.PI ) / 4.5f;
            if ( t < 0.5f )
                return -( Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin(( 20f * t - 11.125f ) * x) ) / 2f;
            return ( Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin(( 20f * t - 11.125f ) * x) ) / 2f + 1f;
        }

        public static float BounceIn(float t) {
            return 1f - BounceOut(1f - t);
        }

        public static float BounceOut(float t) {
            if ( t < ( 1f / 2.75f ) ) {
                return 7.5625f * t * t;
            } else if ( t < ( 2f / 2.75f ) ) {
                return 7.5625f * ( t -= ( 1.5f / 2.75f ) ) * t + 0.75f;
            } else if ( t < ( 2.5f / 2.75f ) ) {
                return 7.5625f * ( t -= ( 2.25f / 2.75f ) ) * t + 0.9375f;
            } else {
                return 7.5625f * ( t -= ( 2.625f / 2.75f ) ) * t + 0.984375f;
            }
        }

        public static float BounceInOut(float t) {
            return t < 0.5f ? BounceIn(t * 2f) * 0.5f : BounceOut(t * 2f - 1f) * 0.5f + 0.5f;
        }

        public static float BackIn(float t) {
            float s = 1.70158f;
            return t * t * ( ( s + 1f ) * t - s );
        }

        public static float BackOut(float t) {
            float s = 1.70158f;
            return --t * t * ( ( s + 1f ) * t + s ) + 1f;
        }

        public static float BackInOut(float t) {
            float s = 1.70158f * 1.525f;
            if ( ( t *= 2f ) < 1f )
                return 0.5f * ( t * t * ( ( s + 1 ) * t - s ) );
            return 0.5f * ( ( t -= 2f ) * t * ( ( s + 1f ) * t + s ) + 2f );
        }
    }
}