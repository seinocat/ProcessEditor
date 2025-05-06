using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Seino.Utils.FastFileReader
{
    /// <summary>
    /// 通用工具
    /// </summary>
    public static class FastFileUtils
    {
        #region BinaryFormatter 的读写方法

        /// <summary>
        /// 简单方法
        /// 大文件（>1 MB）不要用这个
        /// </summary>
        public static T ReadFileByBinaryFormatter<T>(string filePath)
        {

#if FAST_FILE_DEBUG
            long m_StartTime = GetFileTime();
#endif

            T result = default;
            using (var m_FileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var m_BinaryFormatter = new BinaryFormatter();
                result = (T)m_BinaryFormatter.Deserialize(m_FileStream);
            }

#if FAST_FILE_DEBUG
            Debug.Log($"读取文件 {filePath} 完成，耗时：{GetFileTime() - m_StartTime} ms");
#endif
            return result;
        }

        /// <summary>
        /// 简单方法
        /// 大文件（>1 MB）不要用这个
        /// </summary>
        public static void WriteFileByBinaryFormatter(string filePath, object obj)
        {

#if FAST_FILE_DEBUG
            long m_StartTime = GetFileTime();
#endif

            if (obj == null)
                return;

            using (var m_FileStream = File.Create(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(m_FileStream, obj);
            }

#if FAST_FILE_DEBUG
            Debug.Log($"写入文件 {filePath} 完成，耗时：{GetFileTime() - m_StartTime} ms");
#endif
        }

        #endregion

        #region 二进制流读写方法

        internal static void WriteFileByBinary(string filePath, IFileWriter file)
        {
            try
            {
#if FAST_FILE_DEBUG
                long m_StartTime = GetFileTime();
#endif
                using (FileStream fileStream = File.Create(filePath))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                    {
                        var task = file.WriteAsync(binaryWriter);
                        task.Wait();
                    }
                }
#if FAST_FILE_DEBUG
                Debug.Log($"写入文件 {filePath} 完成，耗时：{GetFileTime() - m_StartTime} ms");
#endif
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.DebugException(filePath);
            }
        }

        internal static void ReadFileByBinary(string filePath, IFileReader file)
        {
            try
            {
#if FAST_FILE_DEBUG
                long m_StartTime = GetFileTime();
#endif
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        var task = file.ReadAsync(binaryReader);
                        task.Wait();
                    }
                }
#if FAST_FILE_DEBUG
                Debug.Log($"读取文件 {filePath} 完成，耗时：{GetFileTime() - m_StartTime} ms");
#endif
            }
            catch (AggregateException aggregateException)
            {
                aggregateException.DebugException(filePath);
            }
        }

        #endregion

        #region 对外接口

        /// <summary>
        /// 通过二进制流的方式读取文件
        /// 如路径错误返回 FileNotFoundException
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Task ReadFileByBinaryAsync(string filePath, IFileReader file)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Debug.LogError($"文件路径不合法：{filePath}");
                return Task.FromException(new FileNotFoundException(filePath));
            }

            return Task.Run(() => ReadFileByBinary(filePath, file));
        }
        
        /// <summary>
        /// 通过二进制流的方式写入文件
        /// 如路径不存在 FileNotFoundException
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Task WriteFileByBinaryAsync(string filePath, IFileWriter file)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError($"文件路径不合法：{filePath}");
                return Task.FromException(new FileNotFoundException(filePath));
            }

            return Task.Run(() => WriteFileByBinary(filePath, file));
        }
        
        #endregion

        #region 辅助方法

        private static void DebugException(this AggregateException aggregateException, string filePath)
        {
            string errorStr = "=====> 文件出错 <=====";
            errorStr = $"{errorStr}\n 错误文件：{filePath}\n Message: \n{aggregateException.Message} \n StackTrace: \n {aggregateException.StackTrace}";

            var exs = aggregateException.InnerExceptions;
            int length = exs.Count;
            for (int i = 0; i < length; i++)
            {
                var ex = exs[i];
                errorStr = $"{errorStr}\n InnerExceptions [{i}]: \n Message:\n{ex.Message}\nInnerException: \n{ex.InnerException}\nSource: \n {ex.Source} \n StackTrace: \n {ex.StackTrace}";
            }
            Debug.LogError(errorStr);
        }

        /// <summary>
        /// 获取当前时间（ms）
        /// </summary>
        /// <returns></returns>
        public static long GetFileTime()
        {
            return DateTime.Now.ToFileTime() / 10000;
        }

        #endregion

        #region 重载

        public static void Write(this BinaryWriter writer, float2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static float2 ReadFloat2(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new float2(x, y);
        }

        public static void Write(this BinaryWriter writer, float3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static float3 ReadFloat3(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new float3(x, y, z);
        }

        public static void Write(this BinaryWriter writer, float4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static float4 ReadFloat4(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new float4(x, y, z, w);
        }

        public static void Write(this BinaryWriter writer, float4x4 value)
        {
            writer.Write(value.c0);
            writer.Write(value.c1);
            writer.Write(value.c2);
            writer.Write(value.c3);
        }

        public static float4x4 ReadFloat4x4(this BinaryReader reader)
        {
            float4 c0 = reader.ReadFloat4();
            float4 c1 = reader.ReadFloat4();
            float4 c2 = reader.ReadFloat4();
            float4 c3 = reader.ReadFloat4();
            return new float4x4(c0, c1, c2, c3);
        }

        public static void Write(this BinaryWriter writer, int2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static int2 ReadInt2(this BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            return new int2(x, y);
        }

        public static void Write(this BinaryWriter writer, int3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static int3 ReadInt3(this BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            return new int3(x, y, z);
        }

        public static void Write(this BinaryWriter writer, int4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static int4 ReadInt4(this BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            int w = reader.ReadInt32();
            return new int4(x, y, z, w);
        }

        public static void Write(this BinaryWriter writer, quaternion value)
        {
            writer.Write(value.value);
        }

        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            var value = reader.ReadFloat4();
            return new Quaternion(value.x, value.y, value.z, value.w);
        }

        public static void Write(this BinaryWriter writer, Vector2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        public static void Write(this BinaryWriter writer, Vector3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public static void Write(this BinaryWriter writer, Vector4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static Vector4 ReadVector4(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Vector4(x, y, z, w);
        }

        public static void Write(this BinaryWriter writer, Quaternion value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static Quaternion ReadUnityQuaternion(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        public static void Write(this BinaryWriter writer, Vector2Int value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static Vector2Int ReadVector2Int(this BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            return new Vector2Int(x, y);
        }

        public static void Write(this BinaryWriter writer, Vector3Int value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static Vector3Int ReadVector3Int(this BinaryReader reader)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int z = reader.ReadInt32();
            return new Vector3Int(x, y, z);
        }
        
        public static void Write(this BinaryWriter writer, Color value)
        {
            writer.Write(value.r);
            writer.Write(value.g);
            writer.Write(value.b);
            writer.Write(value.a);
        }
        
        public static Color ReadColor(this BinaryReader reader)
        {
            float r = reader.ReadSingle();
            float g = reader.ReadSingle();
            float b = reader.ReadSingle();
            float a = reader.ReadSingle();
            return new Color(r, g, b, a);
        }
        
        public static void Write(this BinaryWriter writer, Matrix4x4 value)
        {
            writer.Write(value.GetColumn(0));
            writer.Write(value.GetColumn(1));
            writer.Write(value.GetColumn(2));
            writer.Write(value.GetColumn(3));
        }

        public static Matrix4x4 ReadMatrix4x4(this BinaryReader reader)
        {
            Vector4 c0 = reader.ReadVector4();
            Vector4 c1 = reader.ReadVector4();
            Vector4 c2 = reader.ReadVector4();
            Vector4 c3 = reader.ReadVector4();
            return new Matrix4x4(c0, c1, c2, c3);
        }

        public static void WriteHalf(this BinaryWriter writer, half value)
        {
            writer.Write(value.value);
        }

        public static half ReadHalf(this BinaryReader reader)
        {
            ushort x = reader.ReadUInt16();
            half v = half.zero;
            v.value = x;
            return v;
        }

        public static void WriteHalf(this BinaryWriter writer, half2 value)
        {
            writer.WriteHalf(value.x);
            writer.WriteHalf(value.y);
        }

        public static half2 ReadHalf2(this BinaryReader reader)
        {
            half x = reader.ReadHalf();
            half y = reader.ReadHalf();
            return new half2(x, y);
        }

        public static void WriteHalf(this BinaryWriter writer, half3 value)
        {
            writer.WriteHalf(value.x);
            writer.WriteHalf(value.y);
            writer.WriteHalf(value.z);
        }

        public static half3 ReadHalf3(this BinaryReader reader)
        {
            half x = reader.ReadHalf();
            half y = reader.ReadHalf();
            half z = reader.ReadHalf();
            return new half3(x, y, z);
        }

        public static void WriteHalf(this BinaryWriter writer, half4 value)
        {
            writer.WriteHalf(value.x);
            writer.WriteHalf(value.y);
            writer.WriteHalf(value.z);
            writer.WriteHalf(value.w);
        }

        public static half4 ReadHalf4(this BinaryReader reader)
        {
            half x = reader.ReadHalf();
            half y = reader.ReadHalf();
            half z = reader.ReadHalf();
            half w = reader.ReadHalf();
            return new half4(x, y, z, w);
        }

        public static void Write(this BinaryWriter writer, ShadowCastingMode shadowCastingMode)
        {
            writer.Write((int)shadowCastingMode);
        }

        public static ShadowCastingMode ReadShadowCastingMode(this BinaryReader reader)
        {
            int mode = reader.ReadInt32();
            return (ShadowCastingMode)mode;
        }

        #region 特殊：为了解决浮点数精度问题

        /*
         * 一般为了解决浮点数精度问题使用这个方法，适用于极小数值
         * 操作方式是将其放大一定比例，然后四舍五入作为 Int 保存
         * 所以适合 0~1 的浮点数，类似旋转信息的保存适合使用这个
        */

        public const float FloatToIntRatio = 10000.0f;

        public static void WriteSafe(this BinaryWriter writer, float value)
        {
            int ret = (int)Math.Round(value * FloatToIntRatio);
            writer.Write(ret);
        }

        public static float ReadFloatSafe(this BinaryReader reader)
        {
            int v = reader.ReadInt32();
            return v / FloatToIntRatio;
        }

        public static void WriteSafe(this BinaryWriter writer, Quaternion quaternion)
        {
            writer.WriteSafe(quaternion.x);
            writer.WriteSafe(quaternion.y);
            writer.WriteSafe(quaternion.z);
            writer.WriteSafe(quaternion.w);
        }

        public static Quaternion ReadUnityQuaternionSafe(this BinaryReader reader)
        {
            float x = reader.ReadFloatSafe();
            float y = reader.ReadFloatSafe();
            float z = reader.ReadFloatSafe();
            float w = reader.ReadFloatSafe();
            return new Quaternion(x, y, z, w);
        }

        #endregion

        public static void WriteSafe(this BinaryWriter writer, quaternion quaternion)
        {
            writer.WriteSafe(quaternion.value);
        }

        public static quaternion ReadQuaternionSafe(this BinaryReader reader)
        {
            float4 value = reader.ReadFloat4Safe();
            return new quaternion(value);
        }

        public static void WriteSafe(this BinaryWriter writer, float4 value)
        {
            writer.WriteSafe(value.x);
            writer.WriteSafe(value.y);
            writer.WriteSafe(value.z);
            writer.WriteSafe(value.w);
        }

        public static float4 ReadFloat4Safe(this BinaryReader reader)
        {
            float x = reader.ReadFloatSafe();
            float y = reader.ReadFloatSafe();
            float z = reader.ReadFloatSafe();
            float w = reader.ReadFloatSafe();
            return new float4(x, y, z, w);
        }

        public static void Write(this BinaryWriter writer, bool2 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
        }

        public static bool2 ReadBool2(this BinaryReader reader)
        {
            bool x = reader.ReadBoolean();
            bool y = reader.ReadBoolean();
            return new bool2(x, y);
        }

        public static void Write(this BinaryWriter writer, bool3 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }

        public static bool3 ReadBool3(this BinaryReader reader)
        {
            bool x = reader.ReadBoolean();
            bool y = reader.ReadBoolean();
            bool z = reader.ReadBoolean();
            return new bool3(x, y, z);
        }

        public static void Write(this BinaryWriter writer, bool4 value)
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public static bool4 ReadBool4(this BinaryReader reader)
        {
            bool x = reader.ReadBoolean();
            bool y = reader.ReadBoolean();
            bool z = reader.ReadBoolean();
            bool w = reader.ReadBoolean();
            return new bool4(x, y, z, w);
        }

        public static void Write(this BinaryWriter writer, float2x3 value)
        {
            writer.Write(value.c0);
            writer.Write(value.c1);
            writer.Write(value.c2);
        }

        public static float2x3 ReadFloat2x3(this BinaryReader reader)
        {
            float2 c0 = reader.ReadFloat2();
            float2 c1 = reader.ReadFloat2();
            float2 c2 = reader.ReadFloat2();
            return new float2x3(c0, c1, c2);
        }

        public static void Write(this BinaryWriter writer, float3x3 value)
        {
            writer.Write(value.c0);
            writer.Write(value.c1);
            writer.Write(value.c2);
        }

        public static float3x3 ReadFloat3x3(this BinaryReader reader)
        {
            float3 c0 = reader.ReadFloat3();
            float3 c1 = reader.ReadFloat3();
            float3 c2 = reader.ReadFloat3();
            return new float3x3(c0, c1, c2);
        }

        public static void Write(this BinaryWriter writer, float3x4 value)
        {
            writer.Write(value.c0);
            writer.Write(value.c1);
            writer.Write(value.c2);
            writer.Write(value.c3);
        }

        public static float3x4 ReadFloat3x4(this BinaryReader reader)
        {
            float3 x = reader.ReadFloat3();
            float3 y = reader.ReadFloat3();
            float3 z = reader.ReadFloat3();
            float3 w = reader.ReadFloat3();
            return new float3x4(x, y, z, w);
        }

        public static void WriteSafe(this BinaryWriter writer, string data)
        {
            writer.WriteSafeString(data);
        }

        public static void WriteSafeString(this BinaryWriter writer, string data)
        {
            data ??= "";
            writer.Write(data);
        }

        public static void Write(this BinaryWriter writer, string[] strs)
        {
            if (strs == null)
            {
                writer.Write(0);
                return;
            }

            int length = strs.Length;
            writer.Write(length);
            for (int i = 0; i < length; i++)
                writer.WriteSafeString(strs[i]);
        }

        public static string[] ReadStringArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            string[] strs = new string[length];
            for (int i = 0; i < length; i++)
                strs[i] = reader.ReadString();
            return strs;
        }

        public static void Write(this BinaryWriter writer, half[] array)
        {
            if (array == null)
            {
                writer.Write(0);
                return;
            }

            int length = array.Length;
            writer.Write(length);
            for (int i = 0; i < length; i++)
                writer.WriteHalf(array[i]);
        }

        public static half[] ReadHalfArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            half[] array = new half[length];
            for (int i = 0; i < length; i++)
                array[i] = reader.ReadHalf();
            return array;
        }

        public static void Write(this BinaryWriter writer, half4[] array)
        {
            if (array == null)
            {
                writer.Write(0);
                return;
            }

            int length = array.Length;
            writer.Write(length);
            for (int i = 0; i < length; i++)
                writer.WriteHalf(array[i]);
        }

        public static half4[] ReadHalf4Array(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            half4[] array = new half4[length];
            for (int i = 0; i < length; i++)
                array[i] = reader.ReadHalf4();
            return array;
        }

        public static void Write(this BinaryWriter writer, int4[] array)
        {
            if (array == null)
            {
                writer.Write(0);
                return;
            }

            int length = array.Length;
            writer.Write(length);
            for (int i = 0; i < length; i++)
                writer.Write(array[i]);
        }

        public static void Write(this BinaryWriter writer, List<int4> array)
        {
            if (array == null)
            {
                writer.Write(0);
                return;
            }

            int length = array.Count;
            writer.Write(length);
            for (int i = 0; i < length; i++)
                writer.Write(array[i]);
        }

        public static int4[] ReadInt4Array(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            int4[] array = new int4[length];
            for (int i = 0; i < length; i++)
                array[i] = reader.ReadInt4();
            return array;
        }

        public static void Write(this BinaryWriter writer, float2[] array)
        {
            if (array == null)
            {
                writer.Write(0);
                return;
            }

            int length = array.Length;
            writer.Write(length);
            for (int i = 0; i < length; i++)
            {
                var value = array[i];
                writer.Write(value);
            }
        }

        public static float2[] ReadFloat2Array(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            float2[] array = new float2[length];
            for (int i = 0; i < length; i++)
                array[i] = reader.ReadFloat2();
            return array;
        }

        public static void Write(this BinaryWriter writer, float3[] array)
        {
            if (array == null)
            {
                writer.Write(0);
                return;
            }

            int length = array.Length;
            writer.Write(length);
            for (int i = 0; i < length; i++)
            {
                var value = array[i];
                writer.Write(value);
            }
        }

        public static float3[] ReadFloat3Array(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            float3[] array = new float3[length];
            for (int i = 0; i < length; i++)
                array[i] = reader.ReadFloat3();
            return array;
        }

        public static uint[] ReadUIntArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            uint[] array = new uint[length];
            for (int i = 0; i < length; i++)
                array[i] = reader.ReadUInt32();
            return array;
        }

        public static void Write(this BinaryWriter writer, uint[] array)
        {
            if (array == null)
            {
                writer.Write(0);
                return;
            }
            int length = array.Length;
            writer.Write(length);
            for (int i = 0; i < length; i++)
            {
                var value = array[i];
                writer.Write(value);
            }
        }
        #endregion

    }
}