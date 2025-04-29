using System;
using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.Start), ProcessNode, Serializable]
    public class StartEditorNode : ProcessEditorNodeBase
    {
        public override Color color => new Color(1f, 0.42f, 0f);
        
        public override ProcessNodeType Type => ProcessNodeType.Start;
        
        [Input("Config")]
        public ProcessStartPort Input;
        
        [Output("Out", false)]
        public ProcessNodePort Output;
    }
}