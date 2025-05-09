﻿/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeWriter ***/
using System.IO;
using Seino.Utils.FastFileReader;

namespace Process.Editor
{
    public partial class ConditionEditorNode
    {
        public override void WriteNodeData(BinaryWriter writer)
        {
            writer.Write(SuccessID);
            writer.Write(FailID);
        }
    }

    public partial class SequenceEditorNode
    {
        public override void WriteNodeData(BinaryWriter writer)
        {
            writer.Write(IsSequential);
        }
    }

    public partial class WaitTimeEditorNode
    {
        public override void WriteNodeData(BinaryWriter writer)
        {
            writer.Write(Time);
        }
    }

}
