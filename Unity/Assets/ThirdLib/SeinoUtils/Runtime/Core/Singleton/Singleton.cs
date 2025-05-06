using System;

namespace Seino.Utils.Singleton
{
    /// <summary>
    /// 该类单例的生命周期统一为单例创建->退出登陆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        private static T m_Instance;
        
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = Activator.CreateInstance<T>();
                    ((ISingleton)m_Instance).Create();
                }
                return m_Instance;
            }
        }

        void ISingleton.Create()
        {
            OnCreate();
        }

        void ISingleton.Dispose()
        {
            OnDispose();
            m_Instance = null;
        }

        protected virtual void OnCreate() { }
        protected virtual void OnDispose() { }
    }
}