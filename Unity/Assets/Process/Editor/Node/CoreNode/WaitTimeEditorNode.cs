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

        [CustomSetting("")]
        public ulong uid;
        
        [CustomSetting("")]
        public Quaternion rotation;
        
        [CustomSetting("")]
        public Vector3 scale;
        
        [CustomSetting("")]
        public Color color;
    }
}