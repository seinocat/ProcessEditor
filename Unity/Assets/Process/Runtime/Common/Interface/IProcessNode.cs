using System.Collections.Generic;

namespace Process.Runtime
{
    public interface IProcessNode
    {
        void Enter(Dictionary<string, object> streaming = null);
        void Update(float deltaTime);
        void Skip();
        void Dispose();
    }
}
