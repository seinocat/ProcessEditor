/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeWriter ***/
using System.IO;
using Process.Runtime;
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

    public partial class SelectBranchEditorNode
    {
        public override void WriteNodeData(BinaryWriter writer)
        {
            writer.Write(BranchPortL.Count);
            foreach (var element in BranchPortL)
            {
                writer.Write(element);
            }
        }
    }

}
