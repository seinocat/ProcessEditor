/*** 工具自动生成 => Tools/ProcessEditor/GenerateRuntimeNode ***/
using UnityEngine;
using System.Collections.Generic;

namespace Process.Runtime
{
    public partial class ConditionNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Condition;
         public override bool IsStart => false;

         private int m_SuccessID;
         private int m_FailID;

         public override void ReadNodeData(ProcessNodeParam data)
         {
             if(data is ConditionNodeParam paramData)
             {
                 m_SuccessID = paramData.SuccessID;
                 m_FailID = paramData.FailID;
             }
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

    public partial class SequenceNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Sequence;
         public override bool IsStart => false;

         private bool m_IsSequential;

         public override void ReadNodeData(ProcessNodeParam data)
         {
             if(data is SequenceNodeParam paramData)
             {
                 m_IsSequential = paramData.IsSequential;
             }
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

    public partial class WaitTimeNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.WaitTime;
         public override bool IsStart => false;

         private float m_Time;

         public override void ReadNodeData(ProcessNodeParam data)
         {
             if(data is WaitTimeNodeParam paramData)
             {
                 m_Time = paramData.Time;
             }
         }

         protected override void ClearNodeData()
         {
             m_Time = 0f;
         }

         public override void Recycle()
         {
             NodePool<WaitTimeNode>.Recycle(this);
         }

    }

    public partial class EmptyNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Empty;
         public override bool IsStart => false;


         public override void ReadNodeData(ProcessNodeParam data)
         {
             if(data is EmptyNodeParam paramData)
             {
             }
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


         public override void ReadNodeData(ProcessNodeParam data)
         {
             if(data is EndNodeParam paramData)
             {
             }
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


         public override void ReadNodeData(ProcessNodeParam data)
         {
             if(data is SelectBranchNodeParam paramData)
             {
             }
         }

         protected override void ClearNodeData()
         {
         }

         public override void Recycle()
         {
             NodePool<SelectBranchNode>.Recycle(this);
         }

    }

    public partial class StartNode : ProcessNodeBase
    {
         public override ProcessNodeType Type => ProcessNodeType.Start;
         public override bool IsStart => true;


         public override void ReadNodeData(ProcessNodeParam data)
         {
             if(data is StartNodeParam paramData)
             {
             }
         }

         protected override void ClearNodeData()
         {
         }

         public override void Recycle()
         {
             NodePool<StartNode>.Recycle(this);
         }

    }

}
