using System.Collections.Generic;

namespace Process.Runtime
{
    /// <summary>
    /// 节点对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class NodePool<T> where T : ProcessNodeBase, new()
    {
        private static Queue<T> m_Pool = new Queue<T>();
    
        public static T Get()
        {
            return m_Pool.Count > 0 ? m_Pool.Dequeue() : new T();
        }
    
        public static void Recycle(T node)
        {
            node.Dispose();
            m_Pool.Enqueue(node);
        }
        
        public static void Dispose()
        {
            m_Pool.Clear();
        }
    }
}