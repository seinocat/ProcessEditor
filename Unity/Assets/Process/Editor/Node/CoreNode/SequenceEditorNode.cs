using System.Collections.Generic;
using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.Sequence), ProcessNode, System.Serializable]
    public partial class SequenceEditorNode : ProcessEditorNodeBase
    {
        public override Color color => Color.green;

        public override ProcessNodeType Type => ProcessNodeType.Sequence;
        
        [Input("Seq")]
        public SequencePort Input;

        [CustomSetting("顺序执行")]
        public bool IsSequential = true;
        
        [CustomSetting("节点数", false)] 
        public int PortCount = 2;
        
        [Output, SerializeField, HideInInspector]
        public ProcessNodePort Sequences;
        
        [CustomPortBehavior(nameof(Sequences))]
        protected IEnumerable<PortData> OutputPortBehavior(List<SerializableEdge> edges)
        {
            for (int i = 1; i <= PortCount; i++)
            {
                yield return new PortData
                {
                    displayName = $"Port{i}",
                    displayType = typeof(ProcessNodePort),
                    identifier = i.ToString(),
                    acceptMultipleEdges = false,
                };
            }
        }
    }
}