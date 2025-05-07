/*** 工具自动生成 => Tools/ProcessEditor/GenerateRuntimeNode ***/
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Seino.Utils.FastFileReader;

namespace Process.Runtime
{
    public partial class ConditionNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Condition;
         public override bool IsStart => false;

         private int m_SuccessID;
         private int m_FailID;

         public override void ReadNodeData(BinaryReader reader)
         {
             m_SuccessID = reader.ReadInt32();
             m_FailID = reader.ReadInt32();
         }

         protected override void ClearNodeData()
         {
             m_SuccessID = 0;
             m_FailID = 0;
         }

         public override void Recycle()
         {
             NodePool<ConditionNode>.Recycle(this);
         }

    }

    public partial class EmptyNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Empty;
         public override bool IsStart => false;


         public override void ReadNodeData(BinaryReader reader)
         {
         }

         protected override void ClearNodeData()
         {
         }

         public override void Recycle()
         {
             NodePool<EmptyNode>.Recycle(this);
         }

    }

    public partial class EndNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.End;
         public override bool IsStart => false;


         public override void ReadNodeData(BinaryReader reader)
         {
         }

         protected override void ClearNodeData()
         {
         }

         public override void Recycle()
         {
             NodePool<EndNode>.Recycle(this);
         }

    }

    public partial class SelectBranchNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.SelectBranch;
         public override bool IsStart => false;


         public override void ReadNodeData(BinaryReader reader)
         {
         }

         protected override void ClearNodeData()
         {
         }

         public override void Recycle()
         {
             NodePool<SelectBranchNode>.Recycle(this);
         }

    }

    public partial class SequenceNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Sequence;
         public override bool IsStart => false;

         private bool m_IsSequential;

         public override void ReadNodeData(BinaryReader reader)
         {
             m_IsSequential = reader.ReadBoolean();
         }

         protected override void ClearNodeData()
         {
             m_IsSequential = false;
         }

         public override void Recycle()
         {
             NodePool<SequenceNode>.Recycle(this);
         }

    }

    public partial class StartNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Start;
         public override bool IsStart => true;


         public override void ReadNodeData(BinaryReader reader)
         {
         }

         protected override void ClearNodeData()
         {
         }

         public override void Recycle()
         {
             NodePool<StartNode>.Recycle(this);
         }

    }

    public partial class WaitTimeNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.WaitTime;
         public override bool IsStart => false;

         private float m_Time;
         private ulong m_uid;
         private Quaternion m_rotation;
         private Vector3 m_scale;
         private Color m_color;

         public override void ReadNodeData(BinaryReader reader)
         {
             m_Time = reader.ReadSingle();
             m_uid = reader.ReadUInt64();
             m_rotation = reader.ReadQuaternion();
             m_scale = reader.ReadVector3();
             m_color = reader.ReadColor();
         }

         protected override void ClearNodeData()
         {
             m_Time = 0f;
             m_uid = 0;
             m_rotation = Quaternion.identity;
             m_scale = Vector3.zero;
             m_color = Color.black;
         }

         public override void Recycle()
         {
             NodePool<WaitTimeNode>.Recycle(this);
         }

    }

}
