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
        public ulong uid;
        public Quaternion rotation;
        public Vector3 scale;
        public Color color;
        public eFadeType fadeType;
        public List<int> AtkList;
        public List<Vector3> PosList;

        public override void ReadNodeData(BinaryReader reader)
        {
            Time = reader.ReadSingle();
            uid = reader.ReadUInt64();
            rotation = reader.ReadQuaternion();
            scale = reader.ReadVector3();
            color = reader.ReadColor();
            fadeType = (eFadeType)reader.ReadInt32();

            AtkList = new List<int>();
            var AtkListCount = reader.ReadInt32();
            for(int i = 0; i < AtkListCount; i++)
            {
                AtkList.Add(reader.ReadInt32());
            }


            PosList = new List<Vector3>();
            var PosListCount = reader.ReadInt32();
            for(int i = 0; i < PosListCount; i++)
            {
                PosList.Add(reader.ReadVector3());
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
