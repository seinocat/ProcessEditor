using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Seino.Utils.FastFileReader;

namespace Process.Runtime
{
    public class ProcessLoader : IFileReader
    {
        public Dictionary<ulong, ProcessConfig> Configs  { get; private set; }
        
        public Task ReadAsync(BinaryReader reader)
        {
            Configs = new Dictionary<ulong, ProcessConfig>();
            
            var cfgCount = reader.ReadInt32();
            for (int idx = 0; idx < cfgCount; idx++)
            {
                var config           = new ProcessConfig();
                config.NodeDataList  = new List<ProcessNodeData>();
                config.Conditions    = new List<ProcessConditionData>();
                
                config.ProcessId        = reader.ReadUInt64();
                config.TriggerType      = (eTriggerType)reader.ReadUInt32();
                config.MultiProcess     = reader.ReadBoolean();
                config.ConditionCount   = reader.ReadInt32();

                // 读取条件数据
                for (int i = 0; i < config.ConditionCount; i++)
                {
                    ProcessConditionData data = new ProcessConditionData();
                    data.Id          = reader.ReadUInt64();
                    data.IsAnd       = reader.ReadBoolean();
                    
                    config.Conditions.Add(data);
                }
                
                config.NodeCount     = reader.ReadInt32();
                
                // 读取节点数据
                for (int i = 0; i < config.NodeCount; i++)
                {
                    ProcessNodeData nodeData = new ProcessNodeData();
                    
                    // 读取下一节点
                    nodeData.Type           = (ProcessNodeType)reader.ReadInt32();
                    nodeData.Order          = reader.ReadInt32();
                    nodeData.NextNodeCount  = reader.ReadInt32();
                    nodeData.NextNodeOrderList = new List<int>();
                    for (int j = 0; j < nodeData.NextNodeCount; j++)
                    {
                        nodeData.NextNodeOrderList.Add(reader.ReadInt32());
                    }
                    
                    // 读取顺序节点
                    nodeData.IsSequential          = reader.ReadBoolean();
                    nodeData.SeqNodeCount          = reader.ReadInt32();
                    nodeData.SequenceNodeOrderList = new List<int>();
                    for (int j = 0; j < nodeData.SeqNodeCount; j++)
                    {
                        nodeData.SequenceNodeOrderList.Add(reader.ReadInt32());
                    }
                    
                    // 读取节点参数
                    var nodeParam = ProcessNodeParamCreator.Get(nodeData.Type);
                    nodeParam.ReadNodeData(reader);
                    nodeData.Param = nodeParam;
                    
                    config.NodeDataList.Add(nodeData);
                }
                
                Configs.Add(config.ProcessId, config);
            }
            
            return Task.CompletedTask;
        }

        public float Progress { get; }
        public bool IsComplete { get; }
    }
}