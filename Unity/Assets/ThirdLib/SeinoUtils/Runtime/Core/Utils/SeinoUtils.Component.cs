using UnityEngine;

namespace Seino.Utils
{
    /**
     * 组件相关
     */
    public static partial class SeinoUtils
    {
        /// <summary>
        /// 获取组件，没有就添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T RequireComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (!component) component = obj.AddComponent<T>();
            return component;
        }
        
    }
}