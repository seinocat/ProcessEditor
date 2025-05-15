using System;
using System.Collections.Generic;
using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.SelectBranch), ProcessNode, Serializable]
    public partial class SelectBranchEditorNode : ProcessEditorNodeBase
    {
        public override ProcessNodeType Type => ProcessNodeType.SelectBranch;
        
        [Input("In")]
        public ProcessNodePort Input;
        
        [CustomSetting("分支数量", false)] 
        public int PortCount = 2;
        
        [CustomSetting("分支列表")]
        public List<BranchData> BranchPortL = new();
        
        [Output, SerializeField, HideInInspector]
        public ProcessNodePort Branchs;
        
        [CustomPortBehavior(nameof(Branchs))]
        protected IEnumerable<PortData> OutputPortBehavior(List<SerializableEdge> edges)
        {
            for (int i = 0; i < PortCount; i++)
            {
                yield return new PortData
                {
                    displayName = $"Branch{i}",
                    displayType = typeof(ProcessNodePort),
                    identifier = i.ToString(),
                    acceptMultipleEdges = false,
                };
            }
        }
        
        public void AddBranch()
        {
            BranchPortL.Add(new BranchData());
        }

        public void RemoveBranch(int id)
        {
            BranchPortL.RemoveAt(id - 1);
        }

        public void ClearBranch()
        {
            BranchPortL.Clear();
        }

        public override void UpdateForExport()
        {
            foreach (var port in outputPorts)
            {
                if (port == null || string.IsNullOrEmpty(port.portData.identifier))
                    continue;
                
                var index = Math.Max(0, int.Parse(port.portData.identifier));
                if(index <= BranchPortL.Count - 1)
                    SetNextNode(BranchPortL[index], port);
            }
        }
        
        private void SetNextNode(BranchData data, NodePort port)
        {
            if (port.GetEdges().Count > 0)
            {
                if (port.GetEdges()[0].inputNode is ProcessEditorNodeBase node)
                {
                    data.NextID = node.NodeOrder;
                }
            }
            else
            {
                data.NextID = -1;
            }
        }
    }

    [Serializable]
    public class BranchDataNode
    {
        
    }
}