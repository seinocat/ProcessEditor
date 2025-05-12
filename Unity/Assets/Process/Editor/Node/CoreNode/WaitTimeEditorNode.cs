using System;
using System.Collections.Generic;
using Process.Runtime;
using GraphProcessor;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.WaitTime), ProcessNode, System.Serializable]
    public partial class WaitTimeEditorNode : CommonEditorNode
    {
        public override ProcessNodeType Type => ProcessNodeType.WaitTime;
        
        [CustomSetting("时间(秒)")]
        public float Time;
        
        [CustomSetting("测试枚举")]
        public eFadeType FadeType;
    }
}