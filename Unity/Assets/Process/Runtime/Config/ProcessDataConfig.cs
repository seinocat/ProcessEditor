using System;
using System.Collections.Generic;
using Process.Runtime;
using UnityEngine;

[Serializable]
public class ProcessDataBase{}

[Serializable]
public class ProcessConfig
{
    [SerializeField]
    public int ProcessId;
    [SerializeField]
    public bool AutoExecute;
    [SerializeField]
    public bool MultiProcess;
    [SerializeField]
    public List<ProcessConditionConfig> Conditions;
    [SerializeField]
    public List<ProcessNodeData> NodeDataList;
}

[Serializable]
public class ProcessConditionConfig
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
    [SerializeField]
    public int Order;
    [SerializeField]
    public ProcessNodeType Type;
    [SerializeField]
    public List<int> NextNodeOrderList;
    [SerializeField]
    public List<int> SequenceNodeOrderList;
    [SerializeField]
    public bool IsSequential;
}