using System.Collections.Generic;
using System.IO;
using System.Linq;
using GraphProcessor;
using LitJson;
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
        private const string BinaryFileName = "Events.bytes";
        private const string JsonFileName = "Events.json";
        private const string ManifestFileName = "Events.manifest.json";

        [MenuItem("Assets/Open Process Editor")]
        public static async void TestRead()
        {
            ProcessSystem system = new ProcessSystem();
            await system.LoadConfigs();
            _ = system.CreateProcess(1001, null);
        }
        
        /// <summary>
        /// 导出所有流程
        /// </summary>
        public static bool ExportAllProcess()
        {
            var outputFormat = ProcessRuntimeFormatSettings.GetFormat();
            var binaryPath = Path.Combine(Application.streamingAssetsPath, BinaryFileName);

            if (!ExportBinary(binaryPath))
                return false;

            var configs = LoadBinaryConfigs(binaryPath);
            var jsonText = JsonProcessConfigSerializer.Serialize(configs);
            var jsonPath = Path.Combine(Application.streamingAssetsPath, JsonFileName);
            File.WriteAllText(jsonPath, jsonText);

            WriteManifest(outputFormat);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return true;
        }

        private static Dictionary<ulong, ProcessConfig> LoadBinaryConfigs(string binaryPath)
        {
            using var stream = new FileStream(binaryPath, FileMode.Open, FileAccess.Read);
            using var reader = new BinaryReader(stream);
            var loader = new ProcessConfigLoader();
            loader.ReadAsync(reader).GetAwaiter().GetResult();
            return loader.Configs ?? new Dictionary<ulong, ProcessConfig>();
        }

        private static void WriteManifest(ProcessConfigFormat outputFormat)
        {
            var manifest = new ProcessConfigManifest
            {
                Format = outputFormat,
                FileName = outputFormat == ProcessConfigFormat.Json ? JsonFileName : BinaryFileName
            };

            var path = Path.Combine(Application.streamingAssetsPath, ManifestFileName);
            File.WriteAllText(path, JsonMapper.ToJson(manifest));
        }

        private static bool ExportBinary(string binaryPath)
        {
            var writer = FastFileUtils.CreateBinaryWriter(binaryPath);
            var allProcess = ProcessUtils.GetAllProcess();
            writer.Write(allProcess.Count);
             
            foreach (var processGraph in allProcess)
            {
                BaseNode baseNode = processGraph.nodes.Find((node) => node is RootEditorNode);
                if (baseNode == null)
                {
                    Debug.LogError("未配置根节点");
                    continue;
                }

                processGraph.ComputeGraphOrder();
                 
                var outputNodes = baseNode.GetOutputNodeList();
                ProcessConfigEditorNode node = (ProcessConfigEditorNode)outputNodes.Find((n) => n is ProcessConfigEditorNode);
                if (node == null)
                {
                    Debug.LogError("未配置流程配置节点");
                    continue;
                }
                 
                BinaryWriteNodeList(processGraph, node, writer);
            }

            writer.Dispose();
            return true;
        }

        public static void BinaryWriteNodeList(ProcessGraphBase graphBase, ProcessConfigEditorNode nodeData, BinaryWriter writer) 
        {
            writer.Write(nodeData.ProcessId);
            writer.Write((uint)nodeData.TriggerType);
            writer.Write(nodeData.MultiProcess);
            writer.Write(nodeData.Conditions.Count);

            for (int i = 0; i < nodeData.Conditions.Count; i++)
            {
                var condition = nodeData.Conditions[i];
                writer.Write(condition.Id);
                writer.Write(condition.IsAnd);
            }
             
            List<BaseNode> nodes = graphBase.nodes;
            writer.Write(nodes.Count(x=> x is not ProcessEditorNode));
             
            foreach (var baseNode in nodes)
            {
                if (baseNode is ProcessEditorNode)
                    continue;
                 
                BinaryWriteNodeData(baseNode as ProcessEditorNodeBase, writer);
            }
        }

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
