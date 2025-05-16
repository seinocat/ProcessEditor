using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphProcessor;
using Process.Runtime;
using Seino.Utils.FastFileReader;
using UnityEditor;
using UnityEngine;

namespace Process.Editor
{
    /// <summary>
    /// 流程导出工具
    /// </summary>
    public static class ProcessExportUtils
    {
        [MenuItem("Assets/Open Process Editor")]
        public static async void TestRead()
        {
            ProcessSystem system = new ProcessSystem();
            await system.LoadConfigs();
            var process = system.CreateProcess(1001, null);
        }
        
        /// <summary>
        /// 导出所有流程
        /// </summary>
        /// <returns></returns>
        public static bool ExportAllProcess()
        {
            var writer = FastFileUtils.CreateBinaryWriter($"{Application.streamingAssetsPath}/Events.bytes");
            var allProcess = ProcessUtils.GetAllProcess();
            writer.Write(allProcess.Count);
            
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
                ProcessConfigEditorNode node = (ProcessConfigEditorNode)outputNodes.Find((node) => node is ProcessConfigEditorNode);
                if (node == null)
                {
                    Debug.LogError("未配置流程配置节点");
                    continue;
                }
                
                BinaryWriteNodeList(processGraph, node, writer);
            }

            // 释放写入器
            writer.Dispose();
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            return true;
        }

        /// <summary>
        /// 生成节点列表
        /// </summary>
        /// <param name="graphBase"></param>
        /// <param name="nodeData"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static void BinaryWriteNodeList(ProcessGraphBase graphBase, ProcessConfigEditorNode nodeData, BinaryWriter writer) 
        {
            // 写入基本数据
            writer.Write(nodeData.ProcessId);
            writer.Write((uint)nodeData.TriggerType);
            writer.Write(nodeData.MultiProcess);
            writer.Write(nodeData.Conditions.Count);

            // 写入条件数据
            for (int i = 0; i < nodeData.Conditions.Count; i++)
            {
                var condition = nodeData.Conditions[i];
                writer.Write(condition.Id);
                writer.Write(condition.IsAnd);
            }
            
            // 写入节点数据
            List<BaseNode> nodes = graphBase.nodes;
            writer.Write(nodes.Count(x=> x is not ProcessEditorNode));
            
            foreach (var baseNode in nodes)
            {
                if (baseNode is ProcessEditorNode)
                    continue;
                
                BinaryWriteNodeData(baseNode as ProcessEditorNodeBase, writer);
            }
        }

        /// <summary>
        /// 写入节点数据
        /// </summary>
        /// <param name="baseNode"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public static void BinaryWriteNodeData(ProcessEditorNodeBase baseNode, BinaryWriter writer)
        {
            baseNode.UpdateForExport();
            
            var (orderList, seqOrderList, isOrder) = GetNextNodeOrderList(baseNode);
            
            writer.Write((int)baseNode.Type);
            writer.Write(baseNode.NodeOrder);
            writer.Write(orderList.Count);

            for (int i = 0; i < orderList.Count; i++)
            {
                writer.Write(orderList[i]);
            }
            
            writer.Write(isOrder);
            writer.Write(seqOrderList.Count);
            for (int i = 0; i < seqOrderList.Count; i++)
            {
                writer.Write(seqOrderList[i]);
            }
            
            baseNode.WriteNodeData(writer);
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