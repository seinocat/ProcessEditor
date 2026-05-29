using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Seino.Utils.FastFileReader;

namespace Process.Runtime
{
    public sealed class BinaryProcessConfigSerializer : IProcessConfigSerializer
    {
        public async UniTask<Dictionary<ulong, ProcessConfig>> LoadAsync(string filePath)
        {
            ProcessConfigLoader configLoader = new ProcessConfigLoader();
            await FastFileUtils.ReadFileByBinaryAsync(filePath, configLoader);
            return configLoader.Configs ?? new Dictionary<ulong, ProcessConfig>();
        }
    }
}
