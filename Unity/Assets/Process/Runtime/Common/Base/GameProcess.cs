using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Process.Runtime
{
    /// <summary>
    /// 流程实例
    /// </summary>
    public sealed class GameProcess
    {
        /// <summary>
        /// 流程ID
        /// </summary>
        public ulong                    ProcessId;
        
        /// <summary>
        /// 触发类型
        /// </summary>
        public eTriggerType             TriggerType;
        
        /// <summary>
        /// 流程状态
        /// </summary>
        public ProcessStatus            Status { get; private set; }
        
        /// <summary>
        /// 是否停止
        /// </summary>
        public bool                     IsStop => Status is ProcessStatus.Success or  ProcessStatus.FailedBreak;
        
        /// <summary>
        /// 流程完成回调
        /// </summary>
        public Action<ProcessStatus>    OnComplete;
        
        /// <summary>
        /// 流程所有节点
        /// </summary>
        public List<ProcessNodeBase>    ProcessNodes { get; set; }
        
        //开始节点
        private ProcessNodeBase         m_StartNode;
        
        /// <summary>
        /// 资源对象缓存池，用于流程内部资源的持有和回收管理
        /// </summary>
        public Dictionary<string, CacheData>    CacheResDic;
        
        /// <summary>
        /// 资源回收检查回调
        /// </summary>
        public Action<GameProcess>              ResCheckAction;
        
        public GameProcess()
        {
            ProcessNodes = new List<ProcessNodeBase>();
            CacheResDic  = new Dictionary<string, CacheData>();
        }
        
        /// <summary>
        /// 绑定节点
        /// </summary>
        /// <param name="node"></param>
        public void BindNode(ProcessNodeBase node)
        {
            node.OnProcessComplete = OnProcessStop;
            if (node.IsStart) m_StartNode = node;
            ProcessNodes?.Add(node);
        }

        public void Start()
        {
            Debug.Log("Process Start, ProcessId: " + ProcessId);
            Status = ProcessStatus.Running;
            m_StartNode?.Enter();
        }
        
        public void Update(float deltaTime)
        {
            for (int i = 0; i < ProcessNodes?.Count; i++)
                ProcessNodes[i].Update(deltaTime);
        }
        
        /// <summary>
        /// 流程停止(完成和失败)
        /// </summary>
        /// <param name="status"></param>
        private void OnProcessStop(ProcessStatus status)
        {
            Status = status;
            ProcessNodes?.ForEach((node)=> node.OnProcessFinished(status));
            Dispose();
            OnComplete?.Invoke(status);
            OnComplete = null;
            Debug.Log($"Process Stop, ProcessId: {ProcessId}, Status: {status}");
        }  
        
        private void Dispose()
        {
            //回收节点
            foreach (var node in ProcessNodes)
                node.Recycle();
            
            //清空节点列表
            ProcessNodes?.Clear();
            ProcessNodes = null;

            //回收缓存资源
            ResCheckAction?.Invoke(this);
            ResCheckAction = null;
            
            CacheResDic?.Clear();
            CacheResDic = null;
        }
    }

    public class CacheData
    {
        public string ResPath;
        public Object Obj;
        public object Data;
        
        public void Dispose()
        {
            if (Obj != null)
            {
                Object.Destroy(Obj);
                Obj = null;
            }
        }
    }
}