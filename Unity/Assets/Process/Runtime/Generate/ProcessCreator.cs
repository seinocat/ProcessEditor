using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Seino.Utils.FastFileReader;

namespace Process.Runtime
{
    public class ProcessCreator : IFileReader
    {
        public ProcessConfig         Config         { get; private set; }
        public List<ProcessNodeBase> CreatedNodes   { get; private set; }
        
        public List<ProcessNodeData> NodeDataList   { get; private set; }
        
        public List<ProcessConditionData> ConditionData { get; private set; }
        
        public Task ReadAsync(BinaryReader reader)
        {
            Config        = new ProcessConfig();
            CreatedNodes  = new List<ProcessNodeBase>();
            NodeDataList  = new List<ProcessNodeData>();
            ConditionData = new List<ProcessConditionData>();
            
            Config.ProcessId        = reader.ReadInt32();
            Config.AutoExecute      = reader.ReadBoolean();
            Config.MultiProcess     = reader.ReadBoolean();
            Config.ConditionCount   = reader.ReadInt32();

            // 读取条件数据
            for (int i = 0; i < Config.ConditionCount; i++)
            {
                ProcessConditionData data = new ProcessConditionData();
                data.Type        = reader.ReadString();
                data.Id          = reader.ReadUInt64();
                data.Value1      = reader.ReadInt32();
                data.Value2      = reader.ReadInt32();
                data.IsAnd       = reader.ReadBoolean();
                
                ConditionData.Add(data);
            }
            
            Config.NodeCount     = reader.ReadInt32();
            
            // 读取节点数据
            for (int i = 0; i < Config.NodeCount; i++)
            {
                ProcessNodeType nodeType = (ProcessNodeType)reader.ReadInt32();
                ProcessNodeBase node     = ProcessNodePool.Get(nodeType);
                ProcessNodeData nodeData = new ProcessNodeData();
                
                // 读取下一节点
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
                node.ReadNodeData(reader);
                
                CreatedNodes.Add(node);
                NodeDataList.Add(nodeData);
            }
            
            return Task.CompletedTask;
        }

        public float Progress { get; }
        public bool IsComplete { get; }
    }
}