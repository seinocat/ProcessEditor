using GraphProcessor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Process.Editor
{
    [NodeCustomEditor(typeof(SequenceEditorNode))]
    public class SequenceNodeView : BaseNodeView
    {
        private PropertyField PortCount;
        
        public override void Enable()
        {
            base.Enable();
            
            PortCount = this.Q<PropertyField>("PortCount");
            PortCount.RegisterValueChangeCallback(OnPortCount);
        }
        
        private void OnPortCount(SerializedPropertyChangeEvent evt)
        {
            ForceUpdatePorts();
        }
    }
}