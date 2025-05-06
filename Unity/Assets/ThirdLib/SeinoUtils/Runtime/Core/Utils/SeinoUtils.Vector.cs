using UnityEngine;

namespace Seino.Utils
{
    /**
     * vector2, vector3分量转换
     */
    public static partial class SeinoUtils
    {
        #region Vector2
        public static Vector2 X(this Vector2 vector)    => new(vector.x, 0);
        public static Vector2 Y(this Vector2 vector)    => new(0, vector.y);
        public static Vector3 XY0(this Vector2 vector)  => new(vector.x, vector.y, 0);
        public static Vector3 YZ0(this Vector2 vector)  => new(0, vector.x, vector.y);
        public static Vector3 X0Z(this Vector2 vector)  => new(vector.x, 0, vector.y);

        #endregion

        #region Vector3
        public static Vector2 XY(this Vector3 vector)   => new(vector.x, vector.y);
        public static Vector2 XZ(this Vector3 vector)   => new(vector.x, vector.z);
        public static Vector2 YZ(this Vector3 vector)   => new(vector.y, vector.z);
        public static Vector3 XY0(this Vector3 vector)  => new(vector.x, vector.y, 0);
        public static Vector3 YZ0(this Vector3 vector)  => new(0, vector.y, vector.z);
        public static Vector3 X0Z(this Vector3 vector)  => new(vector.x, 0, vector.z);
        public static Vector3 X00(this Vector3 vector)  => new(vector.x, 0, 0);
        public static Vector3 Y00(this Vector3 vector)  => new(0, vector.y, 0);
        public static Vector3 Z00(this Vector3 vector)  => new(0, 0, vector.z);
        public static Vector3 Negative(this Vector3 vector)     => new((vector.x + 180) % 360 - 180, (vector.y + 180) % 360 - 180, (vector.z + 180) % 360 - 180);
        public static Quaternion Rotation(this Vector3 vector)  => Quaternion.Euler(vector);
        
        #endregion
    }
}