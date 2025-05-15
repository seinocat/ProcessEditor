using System;
using System.Collections.Generic;
using UnityEngine;

namespace Process.Runtime
{
    [Serializable, CustomData]
    public class TestClass
    {
        public int Value1;
        public string Value2;
    }
    
    [Serializable, CustomData]
    public class BranchData
    {
        public List<int> Conditions;
        [HideInInspector]
        public int NextID;
    }
}