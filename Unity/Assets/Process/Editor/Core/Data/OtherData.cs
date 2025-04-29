using System;
using System.Collections.Generic;

namespace Process.Editor
{
    [Serializable]
    public class SelectData
    {
        public List<ConditionData> Conditions;
        public int NextNodeID;
    }
    
    [Serializable]
    public class ConditionData
    {
        public int ConditionID;
        public string Desc;
    }
}