using System;
using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{

    [NodeMenuItem((int)ProcessNodeType.End), ProcessNode, Serializable]
    public class EndEditorNode : ProcessEditorNodeBase
    {
        public override Color color => new Color(1f, 0.42f, 0f);

        public override ProcessNodeType Type => ProcessNodeType.End;
        
        [Input("End", true)]
        public ProcessNodePort Input;
    }
}