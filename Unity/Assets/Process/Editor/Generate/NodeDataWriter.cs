/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeWriter ***/
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using Seino.Utils.FastFileReader;

namespace Process.Editor
{
    public class ConditionEditorNodeWriter : IFileWriter
    {
        public ConditionEditorNode Data;
        public float Progress => 0;
        public bool IsComplete { get; }

        public Task WriteAsync(BinaryWriter writer)
        {
            writer.Write(Data.SuccessID);
            writer.Write(Data.FailID);
            return Task.CompletedTask;
        }
    }

    public class SequenceEditorNodeWriter : IFileWriter
    {
        public SequenceEditorNode Data;
        public float Progress => 0;
        public bool IsComplete { get; }

        public Task WriteAsync(BinaryWriter writer)
        {
            writer.Write(Data.IsSequential);
            return Task.CompletedTask;
        }
    }

    public class WaitTimeEditorNodeWriter : IFileWriter
    {
        public WaitTimeEditorNode Data;
        public float Progress => 0;
        public bool IsComplete { get; }

        public Task WriteAsync(BinaryWriter writer)
        {
            writer.Write(Data.Time);
            writer.Write(Data.uid);
            writer.Write(Data.rotation);
            writer.Write(Data.scale);
            writer.Write(Data.color);
            return Task.CompletedTask;
        }
    }

}
