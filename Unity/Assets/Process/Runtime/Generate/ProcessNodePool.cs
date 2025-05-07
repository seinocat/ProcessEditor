/*** 工具自动生成 => Tools/ProcessEditor/GenerateProcessNodePool ***/
using UnityEngine;

namespace Process.Runtime
{
    public static class ProcessNodePool
    {
        public static ProcessNodeBase Get(ProcessNodeType type)
        {
            switch (type)
            {
                case ProcessNodeType.Start:
                    return NodePool<StartNode>.Get();
                case ProcessNodeType.WaitTime:
                    return NodePool<WaitTimeNode>.Get();
                case ProcessNodeType.Sequence:
                    return NodePool<SequenceNode>.Get();
                case ProcessNodeType.SelectBranch:
                    return NodePool<SelectBranchNode>.Get();
                case ProcessNodeType.Condition:
                    return NodePool<ConditionNode>.Get();
                case ProcessNodeType.Empty:
                    return NodePool<EmptyNode>.Get();
                case ProcessNodeType.End:
                    return NodePool<EndNode>.Get();
                default:
                    Debug.LogError($"invalid process node type：{type}");
                    return null;
            }
        }

        public static void DisposeAllPools()
        {
            NodePool<StartNode>.Dispose();
            NodePool<WaitTimeNode>.Dispose();
            NodePool<SequenceNode>.Dispose();
            NodePool<SelectBranchNode>.Dispose();
            NodePool<ConditionNode>.Dispose();
            NodePool<EmptyNode>.Dispose();
            NodePool<EndNode>.Dispose();
        }
    }
}
