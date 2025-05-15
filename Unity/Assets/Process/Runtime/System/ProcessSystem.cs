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
            var nodes           = new List<ProcessNodeBase>();            // 节点列表
            var orderIdToNode   = new Dictionary<int, ProcessNodeBase>(); // 加速查找节点
            
            //先创建所有节点
            foreach (var nodeData in config.NodeDataList)
            {
                ProcessNodeBase processNode = ProcessNodePool.Get(nodeData.Type);
                processNode.Initialize(process, nodeData);
                nodes.Add(processNode);
                orderIdToNode[nodeData.Order] = processNode;
            }
            
            //链接节点
            foreach (var nodeData in config.NodeDataList)
            {
                if (!orderIdToNode.TryGetValue(nodeData.Order, out var curNode))
                {
                    continue;
                }

                //链接下一个节点
                var nextNodeOrders = nodeData.NextNodeOrderList;
                foreach (var nextNodeOrder in nextNodeOrders)
                {
                    if (orderIdToNode.TryGetValue(nextNodeOrder, out var nextNode))
                    {
                        curNode.AddNextNode(nextNode);
                    }
                }
                
                //链接序列节点
                var sequenceNodeOrders = nodeData.SequenceNodeOrderList;
                foreach (var sequenceNodeOrder in sequenceNodeOrders)
                {
                    if (orderIdToNode.TryGetValue(sequenceNodeOrder, out var sequenceNode))
                    {
                        curNode.IsSequenceNode = true;
                        curNode.IsSequential = nodeData.IsSequential;
                        curNode.AddSeqNode(sequenceNode);
                    }
                }
            }
            
            return nodes;
        }
    }
}