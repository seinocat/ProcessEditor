using UnityEngine;

namespace Seino.Utils
{
    /**
     * 图形相关
     */
    public static partial class SeinoUtils
    {
        /// <summary>
        /// 为解决float精度问题
        /// 屏幕坐标转换世界坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public static Vector3 ScreenToWorldPoint(this Camera camera, Vector3 screenPos) {
            double screenX = screenPos.x;
            double screenY = screenPos.y;
            double screenZ = screenPos.z;

            //反齐次除法，求出裁剪空间坐标
            Matrix4x4 pMatrix = camera.projectionMatrix;
            double px = screenX / Screen.width;
            px = (px - 0.5f) * 2f;
            double py = screenY / Screen.height;
            py = (py - 0.5f) * 2f;
            double pz = (-screenZ - pMatrix.m23) / pMatrix.m22;
            double pw = screenZ;
            px *= pw;
            py *= pw;
            
            //裁剪空间到相机空间
            Matrix4x4 pInverseMatrix = camera.projectionMatrix.inverse;
            double vx = (pInverseMatrix.m00 * px + pInverseMatrix.m01 * py + pInverseMatrix.m02 * pz + pInverseMatrix.m03 * pw);
            double vy = (pInverseMatrix.m10 * px + pInverseMatrix.m11 * py + pInverseMatrix.m12 * pz + pInverseMatrix.m13 * pw);
            double vz = (pInverseMatrix.m20 * px + pInverseMatrix.m21 * py + pInverseMatrix.m22 * pz + pInverseMatrix.m23 * pw);
            
            //观察空间到世界空间
            Matrix4x4 vInverseMatrix = camera.worldToCameraMatrix.inverse;
            double x = (vInverseMatrix.m00 * vx + vInverseMatrix.m01 * vy + vInverseMatrix.m02 * vz + vInverseMatrix.m03 * 1);
            double y = (vInverseMatrix.m10 * vx + vInverseMatrix.m11 * vy + vInverseMatrix.m12 * vz + vInverseMatrix.m13 * 1);
            double z = (vInverseMatrix.m20 * vx + vInverseMatrix.m21 * vy + vInverseMatrix.m22 * vz + vInverseMatrix.m23 * 1);
            return new Vector3((float)(x), (float)(y), (float)(z));
        }
        
        /// <summary>
        /// 为解决float精度问题
        /// 世界坐标转屏幕坐标
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static Vector3 WorldToScreenPoint(this Camera camera, Vector3 worldPos) {
            double worldX = worldPos.x;
            double worldY = worldPos.y;
            double worldZ = worldPos.z;

            //世界空间到观察空间
            Matrix4x4 vMatrix = camera.worldToCameraMatrix;
            double vx = (vMatrix.m00 * worldX + vMatrix.m01 * worldY + vMatrix.m02 * worldZ + vMatrix.m03 * 1);
            double vy = (vMatrix.m10 * worldX + vMatrix.m11 * worldY + vMatrix.m12 * worldZ + vMatrix.m13 * 1);
            double vz = (vMatrix.m20 * worldX + vMatrix.m21 * worldY + vMatrix.m22 * worldZ + vMatrix.m23 * 1);
            
            //相机空间到裁剪空间
            Matrix4x4 pMatrix = camera.projectionMatrix;
            double px = (pMatrix.m00 * vx + pMatrix.m01 * vy + pMatrix.m02 * vz + pMatrix.m03 * 1);
            double py = (pMatrix.m10 * vx + pMatrix.m11 * vy + pMatrix.m12 * vz + pMatrix.m13 * 1);
            double pz = (pMatrix.m20 * vx + pMatrix.m21 * vy + pMatrix.m22 * vz + pMatrix.m23 * 1);
            double pw = (pMatrix.m30 * vx + pMatrix.m31 * vy + pMatrix.m32 * vz + pMatrix.m33 * 1);
            
            //齐次除法
            double x = px / pw;
            double y = py / pw;

            //转到0-1的范围
            x = (x * 0.5) + 0.5;
            y = (y * 0.5) + 0.5;
            return new Vector3((float)(x * Screen.width), (float)(y * Screen.height), (float)(-vz));
        }
    }
}