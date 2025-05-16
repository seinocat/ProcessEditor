using System;
using System.Collections.Generic;
using GraphProcessor;
using Process.Runtime;

namespace Process.Editor
{
    [NodeMenuItem("基础流程/流程配置"), ProcessNode, Serializable]
    public class ProcessConfigEditorNode : ProcessEditorNode
    {
        public override string name => "流程配置";
        
        [Input("Root")]
        public EditorPort Input;
        
        [Output("Start", false)]
        public ProcessStartPort Output;

        [CustomSetting("流程ID(必填)")] 
        public ulong ProcessId;
        
        [CustomSetting("允许同时执行")]
        public bool MultiProcess = true;
        
        [CustomSetting("自动触发")] 
        public bool AutoExecute;

        [CustomSetting("条件触发")] 
        public bool NeedCondition;
        
        [CustomSetting("条件列表"), ComplexVisibleIf("ShowConditions")]
        public List<ProcessConditionData> Conditions;
        
        [CustomSetting("备注", false)] 
        public string ProcessDesc;

        public bool ShowConditions() => AutoExecute || NeedCondition;
    }
}