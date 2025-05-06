using System;

namespace Seino.Utils.Tick
{
    public class TickChannel
    {
        public TickStatus Status => m_status;
        
        private float m_deltatime; //update间隔
        private float m_curtime; //本轮时间
        private float m_maxtime; //最大执行时间
        private float m_totaltime; //已执行时间
        private float m_loop; //循环次数，默认-1无限制
        private int m_count; //已执行次数
        
        private Action<float> m_executor; //执行逻辑
        private Func<bool> m_predicate; //条件判断
        private Action m_complete;
        private TickStatus m_status;
        
        public float CurTime => m_curtime;
        public float RemainTime => m_deltatime - m_curtime;
        
        /// <summary>
        /// 创建帧执行器
        /// </summary>
        /// <param name="pre">停止条件</param>
        /// <param name="exe">执行逻辑</param>
        /// <param name="callback">完成回调</param>
        /// <param name="time">最大执行时间</param>
        /// <param name="frame">帧率</param>
        /// <returns></returns>
        public static TickChannel CreateFramer(Func<bool> pre, Action<float> exe, Action callback, float time = -1, int frame = 30)
        {
            TickChannel channel = new TickChannel();
            channel.m_executor = exe;
            channel.m_predicate = pre;
            channel.m_complete = callback;
            channel.m_maxtime = time;
            channel.m_deltatime = 1f / frame;
            return channel;
        }
        
        /// <summary>
        /// 创建时间执行器
        /// </summary>
        /// <param name="pre">停止条件</param>
        /// <param name="exe">执行逻辑</param>
        /// <param name="callback">完成回调</param>
        /// <param name="time">间隔时间</param>
        /// <param name="loop">最大执行次数</param>
        /// <returns></returns>
        public static TickChannel CreateTimer(Func<bool> pre, Action<float> exe, Action callback, float time, int loop = -1)
        {
            TickChannel channel = new TickChannel();
            channel.m_executor = exe;
            channel.m_predicate = pre;
            channel.m_complete = callback;
            channel.m_maxtime = -1;
            channel.m_loop = loop;
            channel.m_deltatime = time;
            return channel;
        }

        /// <summary>
        /// 设置初动时间
        /// </summary>
        public void SetTime(float time)
        {
            this.m_curtime = time;
        }

        public void Update(float deltaTime)
        {
            m_curtime += deltaTime;
            m_totaltime += deltaTime;
            m_status = TickStatus.Running;
            if (m_curtime >= m_deltatime)
            {
                m_curtime -= m_deltatime;
                m_executor(m_deltatime);
                m_count++;
                Check();
            }

            //超过最大时间或次数就中止执行
            if ((m_maxtime > 0f && m_totaltime >= m_maxtime) || (m_loop > 0 && m_count >= m_loop))
            {
                OnComplete();
            }
        }

        private bool Check()
        {
            if (m_predicate?.Invoke()??false)
            {
                OnComplete();
                return true;
            }

            return false;
        }

        public void Play()
        {
            if (!Check())
            {
                m_status = TickStatus.Running;
            }
        }

        public void OnComplete()
        {
            m_status = TickStatus.Complete;
            m_complete?.Invoke();
            m_complete = null;
        }
    }
}