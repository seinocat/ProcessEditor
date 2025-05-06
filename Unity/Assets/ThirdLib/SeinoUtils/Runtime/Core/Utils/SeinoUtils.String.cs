using System.Collections.Generic;
using System.Text;

namespace Seino.Utils
{
    public static partial class SeinoUtils
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        
        /// <summary>
        /// 组合string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Combine(this List<string> value)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Count; i++)
                builder.Append(value[i]);
            return builder.ToString();
        }
        
        /// <summary>
        /// 以指定符号组合string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string Combine(this List<string> value, char symbol)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < value.Count; i++)
            {
                builder.Append(value[i]);
                if (i < value.Count - 1) builder.Append(symbol);
            }
            return builder.ToString();
        }
    }
}