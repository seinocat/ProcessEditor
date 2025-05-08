using System;
using System.Collections.Generic;
using System.IO;

namespace Process.Runtime
{
    [Serializable]
    public class ProcessConfig
    {
        public ulong ProcessId;
        public bool AutoExecute;
        public bool MultiProcess;
        public int ConditionCount;
        public int NodeCount;
        public List<ProcessNodeData> NodeDataList;
        public List<ProcessConditionData> Conditions;
    }

    [Serializable]
    public class ProcessConditionData
    {
        public string Type;
        public ulong Id;
        public int Value1;
        public int Value2;
        public bool IsAnd = true;
    }

    [Serializable]
    public class ProcessNodeData
    {
        public ProcessNodeType Type;
        public int Order;
        public int NextNodeCount;
        public List<int> NextNodeOrderList;
        public bool IsSequential;
        public int SeqNodeCount;
        public List<int> SequenceNodeOrderList;
        public ProcessNodeParam Param;
    }

    [Serializable]
    public class ProcessNodeParam
    {
        public virtual void ReadNodeData(BinaryReader reader){}
    }
}