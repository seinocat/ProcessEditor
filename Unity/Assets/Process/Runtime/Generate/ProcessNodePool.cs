/*** 工具自动生成 => Tools/ProcessEditor/GenerateProcessNodePool ***/
using System;
using UnityEngine;
using System.Collections.Generic;

namespace Process.Runtime
{
    public static class ProcessNodePool
    {
        private static readonly Dictionary<ProcessNodeType, Func<ProcessNodeBase>> m_FactoryMap = new();

        static ProcessNodePool()
        {
             Register<StartNode>(ProcessNodeType.Start);
             Register<WaitTimeNode>(ProcessNodeType.WaitTime);
             Register<SequenceNode>(ProcessNodeType.Sequence);
             Register<SelectBranchNode>(ProcessNodeType.SelectBranch);
             Register<ConditionNode>(ProcessNodeType.Condition);
             Register<EmptyNode>(ProcessNodeType.Empty);
             Register<EndNode>(ProcessNodeType.End);
        }

        private static void Register<T>(ProcessNodeType type) where T : ProcessNodeBase, new()
        {
            m_FactoryMap[type] = () => NodePool<T>.Get();
        }

        public static ProcessNodeBase Get(ProcessNodeType type)
        {
            if (m_FactoryMap.TryGetValue(type, out var creator))
                return creator();

            Debug.LogError($"Invalid process node type: {type}");
            return null;
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

    public static class ProcessNodeParamCreator
    {

        private static readonly Dictionary<ProcessNodeType, Func<ProcessNodeParam>> m_FactoryMap = new();

        static ProcessNodeParamCreator()
        {
             Register<StartNodeParam>(ProcessNodeType.Start);
             Register<WaitTimeNodeParam>(ProcessNodeType.WaitTime);
             Register<SequenceNodeParam>(ProcessNodeType.Sequence);
             Register<SelectBranchNodeParam>(ProcessNodeType.SelectBranch);
             Register<ConditionNodeParam>(ProcessNodeType.Condition);
             Register<EmptyNodeParam>(ProcessNodeType.Empty);
             Register<EndNodeParam>(ProcessNodeType.End);
        }

        private static void Register<T>(ProcessNodeType type) where T : ProcessNodeParam, new()
        {
            m_FactoryMap[type] = () => new T();
        }

        public static ProcessNodeParam Get(ProcessNodeType type)
        {
            if (m_FactoryMap.TryGetValue(type, out var creator))
                return creator();

            Debug.LogError($"Invalid process node type: {type}");
            return null;
        }

    }
}
