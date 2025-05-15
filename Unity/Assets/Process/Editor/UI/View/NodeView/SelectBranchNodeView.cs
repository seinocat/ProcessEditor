using System.Reflection;
using GraphProcessor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Process.Editor
{
    [NodeCustomEditor(typeof(SelectBranchEditorNode))]
    public class SelectBranchNodeView : BaseNodeView
    {
        private PropertyField PortCount;

        private SelectBranchEditorNode EditorNode => nodeTarget as SelectBranchEditorNode;
        
        public override void Enable()
        {
            base.Enable();

            InitElement();
            InitEvent();
        }
        
        private void InitElement()
        {
            PortCount = this.Q<PropertyField>(nameof(PortCount));
        }

        private void InitEvent()
        {
            PortCount.RegisterValueChangeCallback(OnPortCount);
        }

        private void OnPortCount(SerializedPropertyChangeEvent evt)
        {
            var changeCount = System.Math.Abs(evt.changedProperty.intValue - EditorNode.BranchPortL.Count);
            
            if (evt.changedProperty.intValue == 0)
            {
                EditorNode.ClearBranch();
            }
            else if (evt.changedProperty.intValue < EditorNode.BranchPortL.Count)
            {
                EditorNode.BranchPortL.RemoveRange(evt.changedProperty.intValue, EditorNode.BranchPortL.Count - evt.changedProperty.intValue);
            }
            else if (evt.changedProperty.intValue > EditorNode.BranchPortL.Count)
            {
                for (int i = changeCount; i > 0; i--)
                {
                    EditorNode.AddBranch();
                }
            }
            ForceUpdatePorts();
        }
    }
}