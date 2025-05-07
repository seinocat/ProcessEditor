using System.IO;
using Process.Runtime;
using GraphProcessor;
using UnityEngine;

namespace Process.Editor
{
    public abstract class ProcessEditorNodeBase : BaseNode
    {
        public override Color color => Color.cyan;
        
        public override string name => NodeGroupHelper.GetName(Type.GetHashCode());
        
        /// <summary>
        /// 节点顺序
        /// </summary>
        [InspectorName("节点序号"), HideInInspector]
        public int NodeOrder = -1;
        
        /// <summary>
        /// 节点类型
        /// </summary>
        public virtual ProcessNodeType Type { get; }

        /// <summary>
        /// 配置数据序列化方法，需要派生类实现
        /// </summary>
        /// <returns></returns>
        public virtual void WriteNodeData(BinaryWriter writer){}
        
        public virtual void UpdateForExport(){}

        public virtual bool CheckForExport()
        {
            return true;
        }
    }
}