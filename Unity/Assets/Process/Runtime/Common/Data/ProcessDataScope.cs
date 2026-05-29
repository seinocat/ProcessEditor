using System.Collections.Generic;
using UnityEngine;

namespace Process.Runtime
{
    /// <summary>
    /// 节点间流转的数据作用域，封装读写与拷贝语义。
    /// </summary>
    public sealed class ProcessDataScope
    {
        private readonly Dictionary<string, object> m_Data;

        public ProcessDataScope()
        {
            m_Data = new Dictionary<string, object>();
        }

        public ProcessDataScope(Dictionary<string, object> source)
        {
            m_Data = source != null
                ? new Dictionary<string, object>(source)
                : new Dictionary<string, object>();
        }

        public bool TryAdd(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (!m_Data.TryAdd(key, value))
            {
                Debug.LogWarning($"Process streaming data key already exists: {key}");
                return false;
            }

            return true;
        }

        public T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                return default;

            if (!m_Data.TryGetValue(key, out var value))
                return default;

            return value is T casted ? casted : default;
        }

        public Dictionary<string, object> Snapshot()
        {
            return new Dictionary<string, object>(m_Data);
        }

        public void Clear()
        {
            m_Data.Clear();
        }
    }
}
