/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeReader ***/
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using Seino.Utils.FastFileReader;

namespace Process.Runtime
{
    public class ConditionNodeReader : IFileReader
    {
        public float Progress => 0;
        public bool IsComplete { get; }

        public Task ReadAsync(BinaryReader reader)
        {
            reader.ReadInt32();
            reader.ReadInt32();
            return Task.CompletedTask;
        }
    }

    public class SequenceNodeReader : IFileReader
    {
        public float Progress => 0;
        public bool IsComplete { get; }

        public Task ReadAsync(BinaryReader reader)
        {
            reader.ReadBoolean();
            return Task.CompletedTask;
        }
    }

    public class WaitTimeNodeReader : IFileReader
    {
        public float Progress => 0;
        public bool IsComplete { get; }
        // public WaitTimeNode Data;

        public Task ReadAsync(BinaryReader reader)
        {
            reader.ReadSingle();
            reader.ReadUInt64();
            reader.ReadQuaternion();
            reader.ReadVector3();
            reader.ReadColor();
            return Task.CompletedTask;
        }
    }

}
