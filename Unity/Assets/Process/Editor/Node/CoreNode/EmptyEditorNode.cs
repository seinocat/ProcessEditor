using System;
using GraphProcessor;
using Process.Runtime;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.Empty), ProcessNode, Serializable]
    public class EmptyEditorNode : CommonEditorNode
    {
        public override ProcessNodeType Type => ProcessNodeType.Empty;
    }
}