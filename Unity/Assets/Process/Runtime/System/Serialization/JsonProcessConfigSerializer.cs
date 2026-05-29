using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using LitJson;
using UnityEngine;

namespace Process.Runtime
{
    [Serializable]
    public class ProcessConfigJsonRoot
    {
        public List<ProcessConfigJsonItem> Items = new List<ProcessConfigJsonItem>();
    }

    [Serializable]
    public class ProcessConfigJsonItem
    {
        public ulong ProcessId;
        public eTriggerType TriggerType;
        public bool MultiProcess;
        public List<ProcessConditionData> Conditions = new List<ProcessConditionData>();
        public List<ProcessNodeJsonItem> Nodes = new List<ProcessNodeJsonItem>();
    }

    [Serializable]
    public class ProcessNodeJsonItem
    {
        public ProcessNodeType Type;
        public int Order;
        public List<int> NextNodeOrderList = new List<int>();
        public bool IsSequential;
        public List<int> SequenceNodeOrderList = new List<int>();
        public JsonData Param;
    }

    public sealed class JsonProcessConfigSerializer : IProcessConfigSerializer
    {
        public UniTask<Dictionary<ulong, ProcessConfig>> LoadAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Load json process config failed: file not found, path: {filePath}");
                return UniTask.FromResult(new Dictionary<ulong, ProcessConfig>());
            }

            var jsonText = File.ReadAllText(filePath);
            var root = JsonMapper.ToObject<ProcessConfigJsonRoot>(jsonText);
            var result = new Dictionary<ulong, ProcessConfig>();
            if (root?.Items == null)
                return UniTask.FromResult(result);

            for (int i = 0; i < root.Items.Count; i++)
            {
                var src = root.Items[i];
                var config = new ProcessConfig
                {
                    ProcessId = src.ProcessId,
                    TriggerType = src.TriggerType,
                    MultiProcess = src.MultiProcess,
                    Conditions = src.Conditions ?? new List<ProcessConditionData>(),
                    ConditionCount = src.Conditions?.Count ?? 0,
                    NodeDataList = new List<ProcessNodeData>()
                };

                if (src.Nodes != null)
                {
                    for (int j = 0; j < src.Nodes.Count; j++)
                    {
                        var nodeSrc = src.Nodes[j];
                        var nodeParam = ProcessNodeParamCreator.Get(nodeSrc.Type);
                        if (nodeParam == null)
                        {
                            Debug.LogError($"Load json process config failed: invalid node type: {nodeSrc.Type}, processId: {src.ProcessId}, nodeOrder: {nodeSrc.Order}");
                            continue;
                        }

                        if (nodeSrc.Param != null)
                        {
                            var paramJson = nodeSrc.Param.ToJson();
                            nodeParam = (ProcessNodeParam)JsonMapper.ToObject(paramJson, nodeParam.GetType());
                        }

                        config.NodeDataList.Add(new ProcessNodeData
                        {
                            Type = nodeSrc.Type,
                            Order = nodeSrc.Order,
                            NextNodeOrderList = nodeSrc.NextNodeOrderList ?? new List<int>(),
                            NextNodeCount = nodeSrc.NextNodeOrderList?.Count ?? 0,
                            IsSequential = nodeSrc.IsSequential,
                            SequenceNodeOrderList = nodeSrc.SequenceNodeOrderList ?? new List<int>(),
                            SeqNodeCount = nodeSrc.SequenceNodeOrderList?.Count ?? 0,
                            Param = nodeParam
                        });
                    }
                }

                config.NodeCount = config.NodeDataList.Count;
                result[config.ProcessId] = config;
            }

            return UniTask.FromResult(result);
        }

        public static string Serialize(Dictionary<ulong, ProcessConfig> configs)
        {
            var root = new ProcessConfigJsonRoot();
            if (configs != null)
            {
                foreach (var pair in configs)
                {
                    var cfg = pair.Value;
                    var item = new ProcessConfigJsonItem
                    {
                        ProcessId = cfg.ProcessId,
                        TriggerType = cfg.TriggerType,
                        MultiProcess = cfg.MultiProcess,
                        Conditions = cfg.Conditions ?? new List<ProcessConditionData>(),
                        Nodes = new List<ProcessNodeJsonItem>()
                    };

                    if (cfg.NodeDataList != null)
                    {
                        for (int i = 0; i < cfg.NodeDataList.Count; i++)
                        {
                            var node = cfg.NodeDataList[i];
                            item.Nodes.Add(new ProcessNodeJsonItem
                            {
                                Type = node.Type,
                                Order = node.Order,
                                NextNodeOrderList = node.NextNodeOrderList ?? new List<int>(),
                                IsSequential = node.IsSequential,
                                SequenceNodeOrderList = node.SequenceNodeOrderList ?? new List<int>(),
                                Param = node.Param == null
                                    ? new JsonData()
                                    : JsonMapper.ToObject(JsonMapper.ToJson(node.Param, false))
                            });
                        }
                    }

                    root.Items.Add(item);
                }
            }

            return JsonMapper.ToJson(root);
        }
    }
}
