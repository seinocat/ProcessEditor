using System.Collections.Generic;
using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.Condition), ProcessNode, System.Serializable, DiscardNode]
    public partial class ConditionEditorNode : BoolEditorNode
    {
        public override ProcessNodeType Type => ProcessNodeType.Condition;

        // [CustomSetting("条件组"), ListReference(typeof(int), nameof(Conditions))] 
        public List<int> Conditions;

        [CustomSetting("成功节点ID"), HideInInspector]
        public int SuccessID;
        
        [CustomSetting("失败节点ID"), HideInInspector]
        public int FailID;
        
        public ConditionEditorNode()
        {
            onAfterEdgeConnected    += OnEdgeChange;
            onAfterEdgeDisconnected += OnEdgeChange;
        }
        
        private void OnEdgeChange(SerializableEdge edge)
        {
            graph.UpdateComputeOrder();
            foreach (var port in outputPorts)
            {
                switch (port.fieldName)
                {
                    case "Output":
                        SetNextNode(true, port);
                        break;
                    case "Output2":
                        SetNextNode(false, port);
                        break;
                }
            }
        }

        private void SetNextNode(bool isSuccess, NodePort port)
        {
            if (port.GetEdges().Count > 0)
            {
                var node = port.GetEdges()[0].inputNode as ProcessEditorNodeBase;
                if (node == null)
                    return;
                    
                if (isSuccess)  SuccessID   = node.NodeOrder;
                else            FailID      = node.NodeOrder;
            }
            else
            {
                if (isSuccess)  SuccessID   = -1;
                else            FailID      = -1;
            }
        }
    }
}