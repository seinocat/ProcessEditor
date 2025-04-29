using System;
using Process.Runtime;
using Sirenix.OdinInspector;

namespace Process.Editor
{
    [Serializable]
    public class EditorNodeTypeBase
    {
        [ReadOnly]
        public string name;
        [NonSerialized]
        public ProcessNodeType type;
    }
    
    [Serializable]
    public class EditorNodeTypeData : EditorNodeTypeBase
    {
        public int value;
        public string desc;
        public string gourp;
    }
    
    [Serializable]
    public class EditorNetNodeTypeData : EditorNodeTypeBase
    {
        [ReadOnly]
        public int value;
        public string desc;
    }
}