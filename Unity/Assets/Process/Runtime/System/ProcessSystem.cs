using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Seino.Utils.FastFileReader;
using UnityEngine;

namespace Process.Runtime
{
    public class ProcessSystem
    {
        private Dictionary<ulong, ProcessConfig> Configs;

        public async UniTask LoadConfigs()
        {
            ProcessLoader loader = new ProcessLoader();
            await FastFileUtils.ReadFileByBinaryAsync($"{Application.streamingAssetsPath}/Events.bytes", loader);
            Configs = loader.Configs;
        }
        
        /// <summary>
        /// 创建流程实例
        /// </summary>
        /// <param name="res"></param>
        /// <param name="processId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public GameProcess CreateProcess(ulong processId, Action<ProcessStatus> callback)
        {
            //先找配置
            Configs.TryGetValue(processId, out var config);
            if (config == null)
                return null;

            //创建流程实例
            var process = new GameProcess();
            process.ProcessId       = processId;
            process.OnComplete      = callback;
            process.ProcessNodes    = CreateNodeLink(config, process);
        
            return process;
        }
        
    /// <summary>
    /// 创建节点链
    /// </summary>
    /// <param name="config"></param>
    /// <param name="process"></param>
    /// <returns></returns>
    private List<ProcessNodeBase> CreateNodeLink(ProcessConfig config, GameProcess process)
    {
        List<ProcessNodeBase> nodes = new List<ProcessNodeBase>();
        
        //先创建所有节点
        foreach (var nodeData in config.NodeDataList)
        {
            ProcessNodeBase processNode = ProcessNodePool.Get(nodeData.Type);
            processNode.Initialize(process, nodeData);
            nodes.Add(processNode);
        }
        
        //链接节点
        foreach (var nodeData in config.NodeDataList)
        {
            var curNode = nodes.Find((node) => node.OrderId == nodeData.Order);
            if (curNode == null)
            {
                // LogManager.LogError($"create node link error, can't find node by order: {nodeData.Order}");
                continue;
            }

            //链接下一个节点
            var nextNodeOrders = nodeData.NextNodeOrderList;
            foreach (var nextNodeOrder in nextNodeOrders)
            {
                var nextNode = nodes.Find((node) => node.OrderId == nextNodeOrder);
                if (nextNode == null)
                {
                    // LogManager.LogError($"create node link error, can't find next node by order: {nextNodeOrder}");
                    continue;
                }
                curNode.AddNextNode(nextNode);
            }
            
            //链接序列节点
            var sequenceNodeOrders = nodeData.SequenceNodeOrderList;
            foreach (var sequenceNodeOrder in sequenceNodeOrders)
            {
                var sequenceNode = nodes.Find((node) => node.OrderId == sequenceNodeOrder);
                if (sequenceNode == null)
                {
                    // LogManager.LogError($"create node link error, can't find sequence node by order: {sequenceNodeOrder}");
                    continue;
                }
                curNode.IsSequenceNode = true;
                curNode.IsSequential = nodeData.IsSequential;
                curNode.AddSeqNode(sequenceNode);
            }
        }
        
        return nodes;
    }
    }
}