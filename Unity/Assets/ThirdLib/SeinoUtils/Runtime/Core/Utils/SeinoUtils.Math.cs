using UnityEngine;

namespace Seino.Utils
{
    public static partial class SeinoUtils
    {
        /// <summary>
        /// 求直线和平面的交点
        /// p = R + tV
        /// </summary>
        /// <param name="linePoint">直线上的点</param>
        /// <param name="lineDir">直线方向向量(需要归一化)</param>
        /// <param name="planeNormal">平面法线</param>
        /// <param name="planePoint">平面上的点</param>
        /// <returns></returns>
        public static Vector3 LineToPlane(Vector3 linePoint, Vector3 lineDir, Vector3 planeNormal, Vector3 planePoint)
        {
            lineDir = lineDir.normalized;
            float dot1 = Vector3.Dot(planeNormal, planePoint - linePoint);
            float dot2 = Vector3.Dot(planeNormal, lineDir);

            if (dot2 == 0f) return Vector3.zero;
    
            float t = dot1 / dot2;
            return linePoint + t * lineDir;
        }
        
        /// <summary>
        /// 三角余弦定律，求角度(注意不是弧度)
        /// </summary>
        /// <param name="sideA"></param>
        /// <param name="sideB"></param>
        /// <param name="sideC"></param>
        /// <returns></returns>
        public static float CosineTriangle(float sideA, float sideB, float sideC)
        {
            float cosA = Mathf.Clamp((sideB * sideB + sideC * sideC - sideA * sideA) / (2 * sideB * sideC), -1f, 1f);
            return Mathf.Acos(cosA) * Mathf.Rad2Deg;
        }
    }
}