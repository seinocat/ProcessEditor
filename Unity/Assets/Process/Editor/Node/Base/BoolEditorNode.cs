using GraphProcessor;

namespace Process.Editor
{
    public abstract class BoolEditorNode : ProcessEditorNodeBase
    {
        [Input("In")]
        public ProcessNodePort Input;
        
        [Output("True")]
        public ProcessNodePort Output;
        
        [Output("False", false)] 
        public ProcessNodePort Output2;
    }
}