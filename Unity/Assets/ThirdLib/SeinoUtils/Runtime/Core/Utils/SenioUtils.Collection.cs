using System;
using UnityEngine;

namespace Seino.Utils
{
    public static partial class SeinoUtils
    {
        /// <summary>
        /// 移动元素到指定索引,其余元素依次前进或后退
        /// </summary>
        /// <param name="array"></param>
        /// <param name="sourceIndex">当前索引</param>
        /// <param name="targetIndex">目标索引</param>
        /// <typeparam name="T"></typeparam>
        private static void MoveElement<T>(ref T[] array, int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= array.Length)
            {
                Debug.LogError("sourceIndex out of range");
                return;
            }

            if (targetIndex < 0 || targetIndex >= array.Length)
            {
                Debug.LogError("targetIndex out of range");
                return;
            }

            // 保存要移动的元素
            T elementToMove = array[sourceIndex];

            // 移动元素
            if (sourceIndex < targetIndex)
            {
                for (int i = sourceIndex; i < targetIndex; i++)
                {
                    array[i] = array[i + 1];
                }
            }
            else if (sourceIndex > targetIndex)
            {
                for (int i = sourceIndex; i > targetIndex; i--)
                {
                    array[i] = array[i - 1];
                }
            }

            // 将元素放入目标位置
            array[targetIndex] = elementToMove;
        }
    }
}