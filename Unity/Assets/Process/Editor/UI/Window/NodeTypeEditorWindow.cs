using System.Collections.Generic;
using Process.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Process.Editor
{
    public class NodeTypeEditorWindow : OdinEditorWindow
    {
        [LabelText("客户端节点"), TableList(ShowIndexLabels = true, AlwaysExpanded = true)]
        public List<EditorNodeTypeData> EnumDatas;
        
        [MenuItem("Tools/ProcessEditor/NodeTypeEditor")]
        public static void Open()
        {
            var window = GetWindow<NodeTypeEditorWindow>();
            window.OpenWindow();
        }

        public void OpenWindow()
        {
            Init();
            Show();
        }

        private void Init()
        {
            UpdateData();
        }

        [Button("更新数据")]
        public void UpdateData()
        {
            var types = NodeTypeUtils.GetNodeTypes();
            EnumDatas = types;
        }
        
        [Button("客户端重排序")]
        public void SortClient()
        {
            int startIndex = 1000;
            foreach (var type in EnumDatas)
            {
                if (type.type == ProcessNodeType.End) continue;
                type.value = startIndex++;
            }
        }

        [Button("保存")]
        public void Save()
        {
            NodeTypeUtils.WriteType(EnumDatas);
        }
    }


    
}