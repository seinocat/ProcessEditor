using System.Collections.Generic;
using UnityEngine;


namespace Seino.Utils
{
    /**
     * Transform相关
     */
    public static partial class SeinoUtils
    {
        #region 查找

        /// <summary>
        /// 按名称查找节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="targetName"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static Transform GetTransformInChildren(this Transform root, string targetName, SearchOptions option = SearchOptions.Approximate)
        {
            if (option == SearchOptions.Approximate)
                return FindTransform_Approximate(root, targetName);
            return FindTransform_Accurate(root, targetName);
        }

        /// <summary>
        /// 按名称查找所有节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="targetName"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static List<Transform> GetTransformsInChildren(this Transform root, string targetName, SearchOptions option = SearchOptions.Approximate)
        {
            List<Transform> list = new List<Transform>();
            if (option == SearchOptions.Approximate)
                FindTransforms_Approximate(root, targetName, ref list);
            else
                FindTransforms_Accurate(root, targetName, ref list);
            return list;
        }
        
        internal static Transform FindTransform_Approximate(Transform root, string targetName)
        {
            if (root.name.Contains(targetName)) return root;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name.Contains(targetName))
                {
                    return child;
                }

                Transform target = FindTransform_Approximate(child, targetName);
                if (target) return target;
            }

            return null;

        }
        
        internal static Transform FindTransform_Accurate(Transform root, string targetName)
        {
            if (root.name.Equals(targetName)) return root;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name.Equals(targetName))
                {
                    return child;
                }

                Transform target = FindTransform_Accurate(child, targetName);
                if (target) return target;
            }

            return null;
        }
        
        internal static Transform FindTransforms_Approximate(Transform root, string targetName, ref List<Transform> list)
        {
            if (root.name.Contains(targetName)) list.Add(root);

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name.Contains(targetName))
                {
                    list.Add(child);
                }

                Transform target = FindTransforms_Approximate(child, targetName, ref list);
                if (target)  list.Add(target);;
            }

            return null;

        }
        
        internal static Transform FindTransforms_Accurate(Transform root, string targetName, ref List<Transform> list)
        {
            if (root.name.Equals(targetName)) list.Add(root);

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name.Equals(targetName))
                {
                    list.Add(child);
                }

                Transform target = FindTransforms_Accurate(child, targetName, ref list);
                if (target) list.Add(target);
            }

            return null;
        }

        #endregion

        #region 变换

        /// <summary>
        /// 获取参照目标本地坐标和旋转对应的世界坐标和旋转
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static (Vector3 position, Quaternion rotation) GetWorldPositionAndRotation(this Transform transform, Vector3 position, Quaternion rotation)
        {
            Vector3 pos = transform.localToWorldMatrix.MultiplyPoint(position);
            Quaternion rot = transform.rotation * rotation;
            return (pos, rot);
        }
        

        #endregion
        
    }
}