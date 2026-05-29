using System;

namespace Process.Runtime
{
    [Serializable]
    public class ProcessConfigManifest
    {
        public ProcessConfigFormat Format = ProcessConfigFormat.Binary;
        public string FileName = "Events.bytes";
    }
}
