using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.WaitTime), ProcessNode, System.Serializable]
    public partial class WaitTimeEditorNode : CommonEditorNode
    {
        public override ProcessNodeType Type => ProcessNodeType.WaitTime;
        
        [CustomSetting("时间(秒)")]
        public float Time;

        [CustomSetting("Uid")]
        public ulong uid;
        
        [CustomSetting("旋转")]
        public Quaternion rotation;
        
        [CustomSetting("缩放")]
        public Vector3 scale;
        
        [CustomSetting("颜色")]
        public Color color;
    }
}