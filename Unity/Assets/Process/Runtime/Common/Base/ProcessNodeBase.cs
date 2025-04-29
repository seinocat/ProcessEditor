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
        public int                      ProcessId   { get; private set; }
        
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
        public ProcessStatus            Status      { get; private set; }
        
        /// <summary>
        /// 顺序执行状态
        /// </summary>
        public ProcessStatus            SeqStatus   { get; private set; }
        
        /// <summary>
        /// 流程完成回调
        /// </summary>
        public Action<ProcessStatus>    OnProcessComplete;
        
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
        public bool                     IsFinished  => Status is ProcessStatus.Success or ProcessStatus.Failed;
        
        public abstract bool            IsStart     { get; }
        
        /// <summary>
        /// 序列化读取节点数据，需要派生类实现(工具生成)
        /// </summary>
        protected abstract void ReadNodeData();

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
        private Dictionary<string, object> m_StreamingData;
        
        #region 公开方法

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="process"></param>
        /// <param name="data"></param>
        public void Initialize(GameProcess process, ProcessNodeData data)
        {
            //重置节点数据
            Process = process;
            ProcessId = process.ProcessId;
            OrderId = data.Order;
            Status = ProcessStatus.Ready;
            SeqStatus = ProcessStatus.Ready;
            m_IsDirty = false;
            IsSequential = data.IsSequential;
            
            ReadNodeData();
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
            m_StreamingData?.Add(key, value);
        }
        
        /// <summary>
        /// 获取传输数据,如果没有则返回默认值
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetStreamingData<T>(string key)
        {
            if (m_StreamingData.TryGetValue(key, out object value))
                return (T)value;
            return default;
        }
        
        /// <summary>
        /// 执行序列节点
        /// </summary>
        protected async void RunSequence()
        {
            SeqStatus = ProcessStatus.Running;
            
            if (m_SequenceNodes.Count == 0)
            {
                SeqStatus = ProcessStatus.Success;
                return;
            }
            
            //是否按顺序执行
            if (IsSequential)
            {
                foreach (var node in m_SequenceNodes)
                {
                    node.Enter(m_StreamingData);
                    //依次执行节点
                    await UniTask.WaitUntil(() => node.IsFinished);
                }
            }
            else
            {
                m_SequenceNodes.ForEach(node => node.Enter(m_StreamingData));
                //等待序列节点执行完毕
                await UniTask.WaitUntil(() => m_SequenceNodes.All((node) => node.IsFinished));
            }
            
            SeqStatus = ProcessStatus.Success;
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
                m_StreamingData = new Dictionary<string, object>(streaming);
            
            //提供同步和异步两种进入方式，选择一个实现即可
            Status = OnEnter();
            if (Status == ProcessStatus.Running)
                Status = await OnAsyncEnter();
            
            //执行序列节点
            RunSequence();
        }
        
        public void Update(float deltaTime)
        {
            if (m_IsDirty || Status is ProcessStatus.Preparing or ProcessStatus.Ready)  return;
            if (Status == ProcessStatus.Running)                                        Status = OnUpdate(deltaTime);
            if (Status == ProcessStatus.Success && SeqStatus == ProcessStatus.Success)  Exit();
            if (Status == ProcessStatus.Failed)                                         Exit(true);
            
            //超时处理
            OnTimeOut();
        }

        private void Exit(bool isFailed = false)
        {
            OnExit();
            
            // 进入下一个节点
            if (m_NextNodes.Count > 0 && !isFailed)
                m_NextNodes.ForEach((node)=> node.Enter(m_StreamingData));
            
            // 流程结束
            if (m_NextNodes.Count == 0 && Type == ProcessNodeType.End)
                OnProcessComplete?.Invoke(ProcessStatus.Success);
            
            // 流程失败
            if (isFailed)
            {
                //中断流程并抛出错误
                OnProcessComplete?.Invoke(ProcessStatus.Failed);
                Debug.LogError($"ProcessNode error, process id : {ProcessId}, node type : {Type}");
            }
            
            //标记节点不可用
            m_IsDirty = true;
            m_IsTimeOut = false;
        }

        public void Dispose()
        {
            OnProcessComplete = null;
            m_StreamingData?.Clear();
            m_NextNodes?.Clear();
            m_SequenceNodes?.Clear();
            ClearNodeData();
        }

        //此部分由派生类实现
        //Enter方法提供同步和异步两种方式，派生类根据需求选择一个实现即可
        protected virtual ProcessStatus OnEnter()                       => ProcessStatus.Running;
        protected virtual async UniTask<ProcessStatus> OnAsyncEnter()   => ProcessStatus.Running;
        protected virtual ProcessStatus OnUpdate(float deltaTime)       => ProcessStatus.Running;
        protected virtual void OnExit(){}
        public virtual void OnProcessFinished(ProcessStatus status){} 

        #endregion

        #region 内部方法
        


        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="status"></param>
        protected void SetProcessStatus(ProcessStatus status)
        {
            Status = status;
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
    }
}