/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeReader ***/
using System.IO;
using UnityEngine;
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
        public ulong uid;
        public Quaternion rotation;
        public Vector3 scale;
        public Color color;
        public eFadeType fadeType;

        public override void ReadNodeData(BinaryReader reader)
        {
            Time = reader.ReadSingle();
            uid = reader.ReadUInt64();
            rotation = reader.ReadQuaternion();
            scale = reader.ReadVector3();
            color = reader.ReadColor();
            fadeType = (eFadeType)reader.ReadInt32();
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

    public class SelectBranchNodeParam : ProcessNodeParam
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
