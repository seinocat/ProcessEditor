using System;
using System.Collections.Generic;
using Seino.Utils.Singleton;
using UnityEngine;

namespace Seino.Utils.Tick
{
    /// <summary>
    /// 带有中止条件判断的update逻辑执行, 按帧率或者时间间隔执行
    /// Ticker之间是并行执行，Ticker内的Channel按队列执行
    /// </summary>
    public class TickerManager : MonoSingleton<TickerManager>
    {
        private Dictionary<long, Ticker> m_tickers = new();
        private Queue<long> m_updates = new();

        private void Update()
        {
            int count = m_updates.Count;
            while (count-- > 0)
            {
                long id = m_updates.Dequeue();
                if (!m_tickers.TryGetValue(id, out Ticker ticker)) continue;
                if (ticker.Status is TickStatus.Running) ticker.Update(Time.unscaledDeltaTime);
                if (ticker.Status is TickStatus.Complete or TickStatus.Dispose) m_tickers.Remove(id);
                
                m_updates.Enqueue(id);
            }
        }

        #region 创建器

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="time"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Action<float> executor, float time = -1f, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, null, executor, null, time, framerate);
        }
        
        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Action<float> executor, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, null, executor, null, -1, framerate);
        }
        
        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="callback"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Action<float> executor, Action callback, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, null, executor, null, -1, framerate);
        }

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="executor"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Func<bool> predicate, Action<float> executor, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, null, executor, null, -1, framerate);
        }

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="callback"></param>
        /// <param name="time"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Action<float> executor, Action callback, float time = -1f, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, null, executor, callback, time, framerate);
        }

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="executor"></param>
        /// <param name="time"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Func<bool> predicate, Action<float> executor, float time = -1f, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, predicate, executor, null, time, framerate);
        }
        
        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="executor"></param>
        /// <param name="callback"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Func<bool> predicate, Action<float> executor, Action callback, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, predicate, executor, callback, -1, framerate);
        }

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="executor"></param>
        /// <param name="callback"></param>
        /// <param name="time"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(Func<bool> predicate, Action<float> executor, Action callback, float time = -1f, int framerate = 30)
        {
            long id = Guid.NewGuid().GetHashCode();
            return Create(id, predicate, executor, callback, time, framerate);
        }

        /// <summary>
        /// 创建执行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="executor"></param>
        /// <param name="time"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(long id, Action<float> executor, float time = -1f, int framerate = 30)
        {
            return Create(id, null, executor, null, time, framerate);
        }

        /// <summary>
        /// 创建带有中断条件的执行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="predicate"></param>
        /// <param name="executor"></param>
        /// <param name="time"></param>
        /// <param name="framerate"></param>
        /// <returns></returns>
        public Ticker Create(long id, Func<bool> predicate, Action<float> executor, float time = -1f, int framerate = 30)
        {
            return Create(id, predicate, executor, null, time, framerate);
        }

        /// <summary>
        /// 创建带有中断条件的执行
        /// </summary>
        /// <param name="id">相同id的ticker按channel队列执行</param>
        /// <param name="predicate">channel完成条件</param>
        /// <param name="executor">每次update执行逻辑</param>
        /// <param name="callback">channel完成回调</param>
        /// <param name="time">最大执行时间(秒), 默认-1表示无限制</param>
        /// <param name="framerate">执行帧率(默认30帧)</param>
        /// <returns></returns>
        public Ticker Create(long id, Func<bool> predicate, Action<float> executor, Action callback, float time = -1f, int framerate = 30)
        {
            Ticker ticker;
            if (m_tickers.ContainsKey(id))
            {
                ticker = m_tickers[id];
                var channel = TickChannel.CreateFramer(predicate, executor, callback, time, framerate);
                ticker.AddChannel(channel);
            }
            else
            {
                ticker = Ticker.CreateFramer(id, predicate, executor, callback, time, framerate);
                m_tickers.Add(ticker.Id, ticker);
            }
            
            return ticker;
        }
        
        #endregion

        #region 通用方法
        
        /// <summary>
        /// 添加Ticker
        /// </summary>
        /// <param name="ticker"></param>
        public void AddTicker(Ticker ticker)
        {
            if (!m_tickers.ContainsKey(ticker.Id))
            {
                m_tickers.Add(ticker.Id, ticker);
            }
        }
        
        /// <summary>
        /// 添加并执行Ticker
        /// </summary>
        /// <param name="ticker"></param>
        public void ScheduleTicker(Ticker ticker)
        {
            AddTicker(ticker);
            Schedule(ticker.Id);
        }
        
        /// <summary>
        /// 移除Ticker
        /// </summary>
        /// <param name="id"></param>
        public void RemoveTicker(long id)
        {
            if (m_tickers.ContainsKey(id))
            {
                m_tickers.Remove(id);
            }
        }

        /// <summary>
        /// 获取执行数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Ticker GetTicker(long id)
        {
            if (m_tickers.ContainsKey(id))
            {
                return m_tickers[id];
            }
            return null;
        }
        
        /// <summary>
        /// 暂停执行
        /// </summary>
        /// <param name="id"></param>
        public void Pause(long id)
        {
            if (m_tickers.ContainsKey(id))
            {
                m_tickers[id].Pause();
            }
        }

        public void Schedule(long id)
        {
            if (!m_updates.Contains(id))
            {
                m_updates.Enqueue(id);
            }
        }

        /// <summary>
        /// 执行全部
        /// </summary>
        public void ScheduleAll()
        {
            foreach (var kv in m_tickers)
            {
                kv.Value.Schedule();
                if (!m_updates.Contains(kv.Key))
                {
                    m_updates.Enqueue(kv.Key);
                }
            }
        }

        /// <summary>
        /// 暂停全部，慎用，会停止所有逻辑运行
        /// </summary>
        public void PauseAll()
        {
            foreach (var kv in m_tickers)
            {
                kv.Value.Pause();
            }
        }
        
        #endregion
    }
}