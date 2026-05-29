using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Process.Runtime
{
    public interface IProcessConfigSerializer
    {
        UniTask<Dictionary<ulong, ProcessConfig>> LoadAsync(string filePath);
    }
}
