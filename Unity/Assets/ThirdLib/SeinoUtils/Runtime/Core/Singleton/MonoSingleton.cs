using UnityEngine;

namespace Seino.Utils.Singleton
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T m_Instance;
        
        public static T Instance
        {
            get
            {
                if (m_Instance == null) m_Instance = Create();
                return m_Instance;
            }
        }

        public static T Create()
        {
            if (m_Instance != null)
                return m_Instance;

            var go = new GameObject(typeof(T).Name);
            m_Instance = go.AddComponent<T>();
            DontDestroyOnLoad(go);
            return m_Instance;
        }

        public static void Dispose()
        {
            if(!Instance)
            {
                Debug.LogError($"Singleton[{typeof(T).Name}] has been disposed, or not yet be created");
                return;
            }
            
            Destroy(Instance.gameObject);
            m_Instance = null;
        }
    }
}