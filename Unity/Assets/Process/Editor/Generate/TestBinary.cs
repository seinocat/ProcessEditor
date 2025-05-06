using System.IO;
using System.Threading.Tasks;
using Process.Editor;
using Seino.Utils.FastFileReader;
using UnityEngine;

namespace Process.Runtime.Common.Generate
{
    public static class TestBinary
    {
        public static void Test()
        {
            
        }
    }

    public class UINodeWriter : IFileWriter
    {
        public WaitTimeEditorNode Data;
        
        public Task WriteAsync(BinaryWriter writer)
        {
            writer.Write(Data.Time);
            return Task.CompletedTask;
        }

        public float Progress => 0;
        public bool IsComplete { get; }
    }

    public class UINodeReader : IFileReader
    {
        public Task ReadAsync(BinaryReader reader)
        {
            reader.ReadInt32();
            reader.ReadUInt64();
            return Task.CompletedTask;
        }

        public float Progress { get; }
        public bool IsComplete { get; }
    }
}