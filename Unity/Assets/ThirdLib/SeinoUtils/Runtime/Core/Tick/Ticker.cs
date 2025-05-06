using System;
using System.Collections.Generic;
using System.Linq;

namespace Seino.Utils.Tick
{
    /// <summary>
    /// TickChannel的队列
    /// </summary>
    public class Ticker
    {
        public TickStatus Status => m_status;
        public long Id => m_id;
        public Action OnComplete;
        
        private long m_id;
        private Queue<TickChannel> m_channels = new();
        private TickStatus m_status;
        
        /// <summary>
        /// 创建帧执行器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pre"></param>
        /// <param name="exe"></param>
        /// <param name="call"></param>
        /// <param name="time"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static Ticker CreateFramer(long id, Func<bool> pre, Action<float> exe, Action call, float time = -1, int frame = 30)
        {
            Ticker ticker = new Ticker();
            TickChannel channel = TickChannel.CreateFramer(pre, exe, call, time, frame);
            ticker.m_id = id;
            ticker.m_status = TickStatus.Idle;
            ticker.AddChannel(channel);
            return ticker;
        }
        
        /// <summary>
        /// 创建时间执行器
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pre">完成条件</param>
        /// <param name="exe">执行逻辑</param>
        /// <param name="call">完成回调</param>
        /// <param name="time">执行间隔时间</param>
        /// <param name="loop">最大执行次数 默认-1无限制</param>
        /// <returns></returns>
        public static Ticker CreateTimer(long id, Func<bool> pre, Action<float> exe, Action call, float time, int loop = -1)
        {
            Ticker ticker = new Ticker();
            TickChannel channel = TickChannel.CreateTimer(pre, exe, call, time, loop);
            ticker.m_id = id;
            ticker.m_status = TickStatus.Idle;
            ticker.AddChannel(channel);
            return ticker;
        }
        
        public void Update(float deltaTime)
        {
            if (m_status == TickStatus.Dispose)
            {
                m_channels.Clear();
                return;
            }
            
            if (m_channels.Count > 0)
            {
                m_status = TickStatus.Running;
                var channel = m_channels.Peek();
                if (channel.Status == TickStatus.Idle) channel.Play();
                if (channel.Status == TickStatus.Running) channel.Update(deltaTime);
                if (channel.Status == TickStatus.Complete) m_channels.Dequeue();
            }
            else
            {
                Complete();
            }
        }

        /// <summary>
        /// 设置Channel时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="index"></param>
        public void SetTime(float time, int index = 0)
        {
            if (this.m_channels.Count > 0)
            {
                var channel = m_channels.ElementAt(index);
                channel.SetTime(time);
            }
        }

        /// <summary>
        /// 获取当前Ticker本轮已经执行的时间
        /// </summary>
        /// <returns></returns>
        public float GetTime()
        {
            if (this.m_channels.Count > 0)
            {
                var channel = m_channels.Peek();
                return channel.CurTime;
            }

            return -1;
        }
        
        /// <summary>
        /// 获取当前Ticker本轮剩余执行的时间
        /// </summary>
        /// <returns></returns>
        public float GetRemainTime()
        {
            if (this.m_channels.Count > 0)
            {
                var channel = m_channels.Peek();
                return channel.RemainTime;
            }

            return -1;
        }

        public void AddChannel(TickChannel channel)
        {
            m_channels.Enqueue(channel);
        }

        public void AddFrameChannel(Func<bool> pre, Action<float> exe, Action call, float time, int frame)
        {
            AddChannel(TickChannel.CreateFramer(pre, exe, call, time, frame));
        }
        
        public void AddTimeChannel(Func<bool> pre, Action<float> exe, Action call, float time, int loop)
        {
            AddChannel(TickChannel.CreateFramer(pre, exe, call, time, loop));
        }

        public void Schedule()
        {
            m_status = TickStatus.Running;
            TickerManager.Instance.Schedule(m_id);
        }

        public void Pause()
        {
            m_status = TickStatus.Pause;
        }
        
        public void Stop()
        {
            m_status = TickStatus.Stop;
        }
        
        public void Complete()
        {
            m_status = TickStatus.Complete;
            OnComplete?.Invoke();
            OnComplete = null;
        }
        
        public void Dispose()
        {
            m_status = TickStatus.Dispose;
        }
    }
}