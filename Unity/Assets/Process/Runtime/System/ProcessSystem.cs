using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Seino.Utils.FastFileReader;
using UnityEngine;

namespace Process.Runtime
{
    public class ProcessSystem
    {
        /// <summary>
        /// 创建流程实例
        /// </summary>
        /// <param name="res"></param>
        /// <param name="processId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private async UniTask<GameProcess> CreateProcess(GlobalProcessConfig res, int processId, Action<ProcessStatus> callback)
        {
            //先找配置
            var config = res.ProcessList.Find((conf) => conf.ProcessId == processId);
            if (config == null)
                return null;

            //创建流程实例
            var process = new GameProcess();
            process.ProcessId       = processId;
            process.OnComplete      = callback;
            process.ProcessNodes    = await CreateNodeLink(config, process);
        
            return process;
        }
        
    /// <summary>
    /// 创建节点链
    /// </summary>
    /// <param name="config"></param>
    /// <param name="process"></param>
    /// <returns></returns>
    public async UniTask<List<ProcessNodeBase>> CreateNodeLink(ProcessConfig config, GameProcess process)
    {
        List<ProcessNodeBase> nodes = new List<ProcessNodeBase>();
        
        ProcessCreator creator = new ProcessCreator();
        await FastFileUtils.ReadFileByBinaryAsync($"{Application.streamingAssetsPath}/Process_1008.bytes", creator);
        return null;
        
        //先创建所有节点
        foreach (var nodeData in config.NodeDataList)
        {
            ProcessNodeBase processNode = ProcessNodePool.Get(nodeData.Type);
            processNode.Initialize(process, nodeData);
            process.BindNode(processNode);
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