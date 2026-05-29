using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Process.Runtime
{
    /// <summary>
    /// 流程节点基类
    /// </summary>
    public abstract class ProcessNodeBase : IProcessNode
    {
        /// <summary>
        /// 所属流程ID
        /// </summary>
        public ulong                    ProcessId   { get; private set; }
        
        /// <summary>
        /// 流程实例
        /// </summary>
        public GameProcess              Process     { get; private set; }
        
        /// <summary>
        /// 节点序号
        /// </summary>
        public int                      OrderId     { get; private set; }

        /// <summary>
        /// 节点执行状态
        /// </summary>
        public NodeStatus               Status      { get; private set; }
        
        /// <summary>
        /// 顺序执行状态
        /// </summary>
        public NodeStatus               SeqStatus   { get; private set; }
        
        /// <summary>
        /// 节点结束回调，由流程统一裁决是否结束
        /// </summary>
        public Action<ProcessNodeBase>  OnNodeFinished;
        
        /// <summary>
        /// 节点类型
        /// </summary>
        public abstract ProcessNodeType Type        { get; }

        /// <summary>
        /// 是否序列执行
        /// </summary>
        public bool                     IsSequential;

        /// <summary>
        /// 是否为序列节点
        /// </summary>
        public bool                     IsSequenceNode;
        
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool                     IsFinished  => Status is NodeStatus.Success or NodeStatus.Failed or NodeStatus.Skipped;
        
        /// <summary>
        /// 清除节点数据，需要派生类实现(工具生成)
        /// </summary>
        protected abstract void ClearNodeData();
        
        /// <summary>
        /// 回收节点，需要派生类实现(工具生成)
        /// </summary>
        public abstract void Recycle();
        
        /// <summary>
        /// 脏标记
        /// </summary>
        private bool m_IsDirty;
        private int m_SequenceRunVersion;
        
        /// <summary>
        /// 后续节点
        /// </summary>
        private List<ProcessNodeBase> m_NextNodes = new();
        
        /// <summary>
        /// 序列节点
        /// </summary>
        private List<ProcessNodeBase> m_SequenceNodes = new();
        
        /// <summary>
        /// 传输数据，由上一个节点传输过来
        /// </summary>
        private ProcessDataScope m_StreamingData;
        
        #region 公开方法

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="process"></param>
        /// <param name="data"></param>
        public void Initialize(GameProcess process, ProcessNodeData data)
        {
            if (process == null || data == null)
                return;
            
            //初始化节点
            Process      = process;
            ProcessId    = process.ProcessId;
            OrderId      = data.Order;
            Status       = NodeStatus.Ready;
            SeqStatus    = NodeStatus.Ready;
            m_IsDirty    = false;
            m_SequenceRunVersion = 0;
            IsSequential = data.IsSequential;
            
            // 绑定流程
            process.BindNode(this);
            
            // 读取节点数据
            ReadNodeData(data.Param);
        }  
        
        /// <summary>
        /// 添加输出节点
        /// </summary>
        /// <param name="node"></param>
        public void AddNextNode(ProcessNodeBase node)
        {
            m_NextNodes?.Add(node);
        }
        
        /// <summary>
        /// 添加序列节点
        /// </summary>
        /// <param name="node"></param>
        public void AddSeqNode(ProcessNodeBase node)
        {
            m_SequenceNodes?.Add(node);
        }
        
        /// <summary>
        /// 添加传输数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void AddStreamingData(string key, object value)
        {
            m_StreamingData ??= new ProcessDataScope();
            m_StreamingData.TryAdd(key, value);
        }
        
        /// <summary>
        /// 获取传输数据,如果没有则返回默认值
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetStreamingData<T>(string key)
        {
            return m_StreamingData == null ? default : m_StreamingData.Get<T>(key);
        }
        
        /// <summary>
        /// 执行序列节点
        /// </summary>
        protected async void RunSequence()
        {
            int runVersion = ++m_SequenceRunVersion;
            SeqStatus = NodeStatus.Running;
             
            if (m_SequenceNodes.Count == 0)
            {
                SeqStatus = NodeStatus.Success;
                return;
            }
             
            //是否按顺序执行
            if (IsSequential)
            {
                foreach (var node in m_SequenceNodes)
                {
                    if (!IsSequenceRunValid(runVersion))
                        return;

                    node.Enter(m_StreamingData?.Snapshot());
                    //依次执行节点
                    await UniTask.WaitUntil(() => node.IsFinished);

                    if (!IsSequenceRunValid(runVersion))
                        return;

                    if (node.Status == NodeStatus.Failed)
                    {
                        SeqStatus = NodeStatus.Failed;
                        return;
                    }
                }
            }
            else
            {
                var sequenceStreaming = m_StreamingData?.Snapshot();
                m_SequenceNodes.ForEach(node => node.Enter(sequenceStreaming));
                //等待序列节点执行完毕
                await UniTask.WaitUntil(() => m_SequenceNodes.All((node) => node.IsFinished));

                if (!IsSequenceRunValid(runVersion))
                    return;

                if (m_SequenceNodes.Any(node => node.Status == NodeStatus.Failed))
                {
                    SeqStatus = NodeStatus.Failed;
                    return;
                }
            }
             
            if (IsSequenceRunValid(runVersion))
                SeqStatus = NodeStatus.Success;
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CacheData GetCacheResData(string key)
        {
            if (Process == null || Process.CacheResDic == null)
                return null;
            
            Process.CacheResDic.TryGetValue(key, out CacheData data);
            return data;
        }
        
        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void AddCacheResData(string key, CacheData data)
        {
            if (Process == null || Process.CacheResDic == null)
                return;

            if (!Process.CacheResDic.TryAdd(key, data))
            {
                Debug.LogError($"Cache data key : {key} already exists");
            }
        }
        
        /// <summary>
        /// 移除缓存数据
        /// </summary>
        /// <param name="key"></param>
        public void RemoveCacheResData(string key)
        {
            if (Process == null || Process.CacheResDic == null)
                return;

            if (Process.CacheResDic.ContainsKey(key))
            {
                Process.CacheResDic.Remove(key);
            }
        }
        
        #endregion
        
        #region 生命周期
        
        public async void Enter(Dictionary<string, object> streaming = null)
        {
            Debug.Log($"Enter node, process id : {ProcessId}, node type : {Type}");
            
            m_startTime = Time.time;
            
            if (streaming != null) 
                m_StreamingData = new ProcessDataScope(streaming);
            else
                m_StreamingData ??= new ProcessDataScope();
            
            Status = await OnEnter();
            
            //执行序列节点
            RunSequence();
        }
        
        public void Update(float deltaTime)
        {
            if (m_IsDirty || Status is NodeStatus.Preparing or NodeStatus.Ready)  return;
            if (Status == NodeStatus.Running)                                      Status = OnUpdate(deltaTime);
            if (IsStateFinished() && IsSeqStateFinished())                              Exit();
            if (Status == NodeStatus.Failed)                                            Exit();
            
            //超时处理
            OnTimeOut();
        }

        public void Skip()
        {
            if (m_IsDirty || IsFinished)
                return;

            m_SequenceRunVersion++;
            Status = OnSkip();
            SeqStatus = NodeStatus.Skipped;

            if (IsStateFinished() || Status == NodeStatus.Failed)
                Exit();
        }

        private void Exit()
        {
            OnExit();
            
            // 进入下一个节点
            if (m_NextNodes.Count > 0 && Status != NodeStatus.Failed)
                m_NextNodes.ForEach((node)=> node.Enter(m_StreamingData?.Snapshot()));

            //标记节点不可用
            m_IsDirty   = true;
            m_IsTimeOut = false;

            if (Status == NodeStatus.Failed)
                Debug.LogError($"ProcessNode error, process id : {ProcessId}, node type : {Type}");

            OnNodeFinished?.Invoke(this);
        }

        public void Dispose()
        {
            m_SequenceRunVersion++;
            OnNodeFinished = null;
            m_StreamingData?.Clear();
            m_StreamingData = null;
            m_NextNodes?.Clear();
            m_SequenceNodes?.Clear();
            ClearNodeData();
        }

        //此部分由派生类实现
        protected virtual UniTask<NodeStatus> OnEnter()              => UniTask.FromResult(NodeStatus.Running);
        protected virtual NodeStatus OnUpdate(float deltaTime)       => NodeStatus.Running;
        protected virtual NodeStatus OnSkip()                        => NodeStatus.Success;   
        protected virtual void OnExit(){}
        public virtual void OnProcessFinished(ProcessStatus status){} 

        #endregion

        #region 内部方法

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="status"></param>
        protected void SetProcessStatus(NodeStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// 当前节点是否完成(失败跳过也算完成)
        /// </summary>
        /// <returns></returns>
        private bool IsStateFinished()
        {
            return Status is NodeStatus.Success or NodeStatus.Skipped;
        }

        /// <summary>
        /// 序列节点是否完成(失败跳过也算完成)
        /// </summary>
        /// <returns></returns>
        private bool IsSeqStateFinished()
        {
            return SeqStatus is NodeStatus.Success or NodeStatus.Skipped;
        }

        private bool IsSequenceRunValid(int runVersion)
        {
            return runVersion == m_SequenceRunVersion && !m_IsDirty;
        }

        #endregion

        #region 异常提示

        private readonly float m_timeout = 10f;
        private bool m_IsTimeOut;
        private float m_startTime;
        
        private void OnTimeOut()
        {
            if (!m_IsTimeOut && Time.time - m_startTime >= m_timeout)
            {
                m_IsTimeOut = true;
                //提示当前节点运行时间过长
                Debug.LogWarning($"ProcessNode timeout, process id : {ProcessId}, node type : {Type}");
            }
        }

        #endregion

        #region 节点参数读取

        public virtual void ReadNodeData(ProcessNodeParam data){}

        #endregion
    }
}
