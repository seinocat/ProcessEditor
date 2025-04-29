using System;
using System.Collections.Generic;
using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{
    [NodeMenuItem((int)ProcessNodeType.SelectBranch), ProcessNode, Serializable, DiscardNode]
    public partial class SelectBranchEditorNode : ProcessEditorNodeBase
    {
        public override ProcessNodeType Type => ProcessNodeType.SelectBranch;
        
        [Input("In")]
        public ProcessNodePort Input;
        
        [CustomSetting("分支端口", false)] 
        public int PortCount = 2;
        
        // [CustomSetting("分支列表"), ListReference(typeof(BranchData), nameof(BranchDatas))]
        public List<BranchData> BranchDatas = new List<BranchData>();
        
        [Output, SerializeField, HideInInspector]
        public ProcessNodePort Branchs;
        
        [CustomPortBehavior(nameof(Branchs))]
        protected IEnumerable<PortData> OutputPortBehavior(List<SerializableEdge> edges)
        {
            for (int i = 0; i < PortCount; i++)
            {
                yield return new PortData
                {
                    displayName = $"分支{i}",
                    displayType = typeof(ProcessNodePort),
                    identifier = i.ToString(),
                    acceptMultipleEdges = false,
                };
            }
        }
        
        public void AddBranch()
        {
            BranchDatas.Add(new BranchData());
        }

        public void RemoveBranch(int id)
        {
            BranchDatas.RemoveAt(id - 1);
        }

        public void ClearBranch()
        {
            BranchDatas.Clear();
        }

        public override void UpdateForExport()
        {
            foreach (var port in outputPorts)
            {
                if (port == null || string.IsNullOrEmpty(port.portData.identifier))
                    continue;
                
                var index = Math.Max(0, int.Parse(port.portData.identifier));
                if(index <= BranchDatas.Count - 1)
                    SetNextNode(BranchDatas[index], port);
            }
        }
        
        private void SetNextNode(BranchData data, NodePort port)
        {
            if (port.GetEdges().Count > 0)
            {
                data.NextID = port.GetEdges()[0].inputNode.computeOrder;
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