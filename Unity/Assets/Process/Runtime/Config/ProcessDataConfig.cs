using System;
using System.Collections.Generic;

namespace Process.Runtime
{
    [Serializable]
    public class ProcessConfig
    {
        public int ProcessId;
        public bool AutoExecute;
        public bool MultiProcess;
        public int ConditionCount;
        public List<ProcessConditionData> Conditions;
        public int NodeCount;
        public List<ProcessNodeData> NodeDataList;
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
        public int Order;
        public ProcessNodeType Type;
        public int NextNodeCount;
        public List<int> NextNodeOrderList;
        public int SeqNodeCount;
        public List<int> SequenceNodeOrderList;
        public bool IsSequential;
    }
}