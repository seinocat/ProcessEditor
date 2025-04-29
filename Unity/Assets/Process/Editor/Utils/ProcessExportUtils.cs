using System;
using System.Collections.Generic;
using GraphProcessor;
using Process.Runtime;
using ProcessEditor;
using UnityEditor;
using UnityEngine;

namespace Process.Editor
{
    /// <summary>
    /// 流程导出工具
    /// </summary>
    public static class ProcessExportUtils
    {
        /// <summary>
        /// 导出所有流程
        /// </summary>
        /// <returns></returns>
        public static bool ExportAllProcess()
        {
            GlobalProcessConfig globalProcessConfig = ScriptableObject.CreateInstance<GlobalProcessConfig>();
            var allProcess = ProcessUtils.GetAllProcess();
            foreach (var processGraph in allProcess)
            {
                //根节点
                BaseNode baseNode = processGraph.nodes.Find((node) => node is RootEditorNode);
                if (baseNode == null)
                {
                    Debug.LogError("未配置根节点");
                    continue;
                }

                processGraph.ComputeGraphOrder();
                
                var outputNodes = baseNode.GetOutputNodeList();
                ProcessConfigEditorNode processConfigEditorNode = (ProcessConfigEditorNode)outputNodes.Find((node) => node is ProcessConfigEditorNode);
                if (processConfigEditorNode == null)
                {
                    Debug.LogError("未配置流程配置节点");
                    continue;
                }

                ProcessConfig processConfig = new ProcessConfig();
                processConfig.ProcessId = processConfigEditorNode.ProcessId;
                processConfig.AutoExecute = processConfigEditorNode.AutoExecute;
                processConfig.MultiProcess = processConfigEditorNode.MultiProcess;
                
                processConfig.NodeDataList = GenerateNodeList(processGraph);
            
                globalProcessConfig.ProcessList.Add(processConfig);
            }
            
            AssetDatabase.CreateAsset(globalProcessConfig, GlobalPathConfig.ConfigExportPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            return true;
        }

        /// <summary>
        /// 生成节点列表
        /// </summary>
        /// <param name="graphBase"></param>
        /// <returns></returns>
        public static List<ProcessNodeData> GenerateNodeList(ProcessGraphBase graphBase)
        {
            List<ProcessNodeData> nodeList = new List<ProcessNodeData>();
            List<BaseNode> nodes = graphBase.nodes;
            foreach (var baseNode in nodes)
            {
                if (baseNode is EditorEditorNode)
                    continue;
                
                var nodeData = GenerateNodeData(baseNode as ProcessEditorNodeBase);
                if (nodeData.Type != ProcessNodeType.Sequence)
                    nodeList.Add(nodeData);
            }
            return nodeList;
        }
        
        /// <summary>
        /// 生成节点数据
        /// </summary>
        /// <param name="baseEditorNode"></param>
        /// <returns></returns>
        public static ProcessNodeData GenerateNodeData(ProcessEditorNodeBase baseEditorNode)
        {
            ProcessNodeData processNodeData = new ProcessNodeData();
            processNodeData.Order = baseEditorNode.NodeOrder;
            processNodeData.Type = baseEditorNode.Type;
            // processNodeData.Data = baseEditorNode.WriteNodeData();
            var (orderList, seqOrderList, isOrder) = GetNextNodeOrderList(baseEditorNode);
            processNodeData.NextNodeOrderList = orderList;
            processNodeData.SequenceNodeOrderList = seqOrderList;
            processNodeData.IsSequential = isOrder;
            return processNodeData;
        }

        /// <summary>
        /// 获取后续节点列表
        /// </summary>
        /// <param name="baseEditorNode"></param>
        /// <returns></returns>
        public static (List<int>, List<int>, bool) GetNextNodeOrderList(ProcessEditorNodeBase baseEditorNode)
        {
            List<int> orderList = new List<int>();
            List<int> seqOrderList = new List<int>();
            bool isOrder = false;

            List<BaseNode> nextNodes = baseEditorNode.GetOutputNodeList();
            foreach(var baseNode1 in nextNodes)
            {
                var node = (ProcessEditorNodeBase)baseNode1;
                if(node is SequenceEditorNode seqNode)
                {
                    List<BaseNode> seqNextNodes = node.GetOutputNodeList();
                    foreach(var baseNode2 in seqNextNodes)
                    {
                        var seqNextNode = (ProcessEditorNodeBase)baseNode2;
                        seqOrderList.Add(seqNextNode.NodeOrder);
                    }

                    isOrder = seqNode.IsSequential;
                }else
                {
                    orderList.Add(node.NodeOrder);
                }
            }
            return (orderList, seqOrderList, isOrder);
        }
    }
}