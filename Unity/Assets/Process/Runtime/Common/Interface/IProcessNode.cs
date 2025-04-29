namespace Process.Runtime
{
    public interface IProcessNode
    {
        protected ProcessStatus OnEnter()  => ProcessStatus.Running;
        protected ProcessStatus OnUpdate() => ProcessStatus.Success;
        protected void OnExit(){}
    }
}