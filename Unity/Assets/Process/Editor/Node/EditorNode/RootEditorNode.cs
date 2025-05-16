using System;
using Process.Runtime;
using GraphProcessor;
namespace Process.Editor
{
    [NodeMenuItem("基础流程/根节点"), ProcessNode, Serializable]
    public class RootEditorNode : ProcessEditorNode
    {
        public override string name => "根节点";
        
        [Output("Root")]
        public EditorPort Output;
    }
}