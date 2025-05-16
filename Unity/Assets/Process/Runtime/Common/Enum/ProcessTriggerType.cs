using System;
using UnityEngine;

namespace Process.Runtime
{
    [Flags]
    public enum eTriggerType
    {
        [InspectorName("无")]
        None        = 0,
        [InspectorName("条件")]
        Condition   = 1 << 0,
        [InspectorName("进入")]
        Enter       = 1 << 1,
    }
}