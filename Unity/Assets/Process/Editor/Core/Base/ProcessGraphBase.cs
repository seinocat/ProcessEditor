using System.Collections.Generic;
using GraphProcessor;

namespace Process.Editor
{
    public class ProcessGraphBase : BaseGraph
    {
        public long Timestamp = TimeHelper.GetTimestamp();
        public int  OpenCount;
        public ProcessGraphWindow ProcessWindow;

        public List<T> GetNode<T>() where T : ProcessEditorNodeBase
        {
            List<T> list = new List<T>();
            foreach (var node in this.nodes)
            {
                if (node is T @base)
                {
                    list.Add(@base);
                }
            }

            return list;
        }
        
        /// <summary>
        /// 计算Graph的执行Order
        /// </summary>
        public void ComputeGraphOrder()
        {
            foreach (var node in nodes)
            {
                ((ProcessEditorNodeBase)node).NodeOrder = -1;
            }
            
            foreach (var node in nodes)
            {
                if (node is not ProcessConfigEditorNode eventFlag) 
                    continue;

                if (eventFlag.GetOutputNodeList().Count <= 0) 
                    continue;
                
                var startNode = eventFlag.GetOutputNodeList()[0];
                if (startNode is not StartEditorNode initNode) 
                    continue;
                
                int order = 1;
                initNode.NodeOrder = order;
                
                ComputeNodeOrder(initNode, ref order);
            }
        }
        
        /// <summary>
        /// 计算节点的执行Order
        /// </summary>
        /// <param name="editorNodeBase"></param>
        /// <param name="order"></param>
        private void ComputeNodeOrder(ProcessEditorNodeBase editorNodeBase, ref int order)
        {
            foreach (var output in editorNodeBase.GetOutputNodeList())
            {
                var outputNode = output as ProcessEditorNodeBase;
                if (outputNode == null) 
                    continue;
                
                outputNode.NodeOrder = ++order;
                if (outputNode.GetOutputNodeList().Count > 0)
                {
                    ComputeNodeOrder(outputNode, ref order);
                }
            }
        }
    }
}