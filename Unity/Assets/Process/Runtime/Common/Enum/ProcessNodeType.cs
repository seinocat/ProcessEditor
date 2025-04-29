using System;
using UnityEngine;

namespace Process.Runtime
{
    // 注意：自动生成代码，请勿手动修改
    [Serializable]
    public enum ProcessNodeType
    {
        [InspectorName("开始")]
        Start = 1000,

        [InspectorName("等待时间")]
        WaitTime = 1001,

        [InspectorName("顺序节点")]
        Sequence = 1002,

        [InspectorName("选择分支")]
        SelectBranch = 1003,

        [InspectorName("条件节点")]
        Condition = 1004,

        [InspectorName("空节点")]
        Empty = 1005,

        [InspectorName("结束")]
        End = 9999999,

    }
}
