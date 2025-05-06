using UnityEngine;

namespace Seino.Utils
{
    /**
     * 设置相关
     */
    
    public enum SearchOptions
    {
        /// <summary>
        /// 近似查找
        /// </summary>
        Approximate,
        /// <summary>
        /// 精准查找
        /// </summary>
        Accurate
    }
    
    public enum LerpType
    {
        [InspectorName("线性插值")]
        Linear,
        [InspectorName("简单阻尼插值")]
        SampleDamper,
        [InspectorName("阻尼插值")]
        Damper
    }
}