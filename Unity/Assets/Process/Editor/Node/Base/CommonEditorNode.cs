using GraphProcessor;

namespace Process.Editor
{
    public abstract class CommonEditorNode : ProcessEditorNodeBase
    {
        [Input("In")]
        public ProcessNodePort Input;
        
        [Output("Out", false)]
        public ProcessNodePort Output;
        
        [Output("Seq")]
        public SequencePort Sequence;
    }
}