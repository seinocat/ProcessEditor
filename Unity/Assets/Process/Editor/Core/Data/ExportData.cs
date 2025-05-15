using System;
using System.Collections.Generic;
using Process.Runtime;
using UnityEngine;

namespace Process.Editor
{
    [Serializable, ExportData]
    public class SequenceData
    {
        [HideInInspector]
        public int NextID;
    }
    
    [Serializable, ExportData]
    public class RandomBranchData
    {
        public int Weight;
        [HideInInspector]
        public int NextID;
    }
}

