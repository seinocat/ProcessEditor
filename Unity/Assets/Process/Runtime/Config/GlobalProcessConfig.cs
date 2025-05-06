using System;
using System.Collections.Generic;
using UnityEngine;

namespace Process.Runtime
{
    [Serializable]
    [CreateAssetMenu(menuName = "Process/GlobalProcessConfig", order = 2)]
    public class GlobalProcessConfig : ScriptableObject
    {
        public List<ProcessConfig> ProcessList = new();
    }
}



