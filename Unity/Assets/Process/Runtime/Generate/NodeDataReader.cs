/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeReader ***/
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using Seino.Utils.FastFileReader;

namespace Process.Runtime
{
    public class ConditionNodeParam : ProcessNodeParam
    {
        public int SuccessID;
        public int FailID;

        public override void ReadNodeData(BinaryReader reader)
        {
            SuccessID = reader.ReadInt32();
            FailID = reader.ReadInt32();
        }
    }

    public class SequenceNodeParam : ProcessNodeParam
    {
        public bool IsSequential;

        public override void ReadNodeData(BinaryReader reader)
        {
            IsSequential = reader.ReadBoolean();
        }
    }

    public class WaitTimeNodeParam : ProcessNodeParam
    {
        public float Time;

        public override void ReadNodeData(BinaryReader reader)
        {
            Time = reader.ReadSingle();
        }
    }

    public class SelectBranchNodeParam : ProcessNodeParam
    {
        public List<BranchData> BranchPortL;

        public override void ReadNodeData(BinaryReader reader)
        {

            BranchPortL = new List<BranchData>();
            var BranchPortLCount = reader.ReadInt32();
            for(int i = 0; i < BranchPortLCount; i++)
            {
                BranchPortL.Add(reader.ReadBranchData());
            }

        }
    }

    public class EmptyNodeParam : ProcessNodeParam
    {

        public override void ReadNodeData(BinaryReader reader)
        {
        }
    }

    public class EndNodeParam : ProcessNodeParam
    {

        public override void ReadNodeData(BinaryReader reader)
        {
        }
    }

    public class StartNodeParam : ProcessNodeParam
    {

        public override void ReadNodeData(BinaryReader reader)
        {
        }
    }

}
