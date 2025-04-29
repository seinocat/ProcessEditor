using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using GraphProcessor;
using Process.Runtime;
using ProcessEditor;
using UnityEditor;
using UnityEngine;

namespace Process.Editor
{
    public static class NodeTypeUtils
    {
        public static Dictionary<int, NodeGroupData> NodeGroups = new();
        
        public static void LoadConfig()
        {
            NodeGroups = new Dictionary<int, NodeGroupData>();
            var config = AssetDatabase.LoadAssetAtPath<TextAsset>(GlobalPathConfig.NodeGroupConfigAssetPath)?.text;
            NodeGroup group = JsonUtility.FromJson<NodeGroup>(config);
            foreach (var data in group.NodeConfig)
                NodeGroups.Add(data.ID, data);
        }
        
        public static List<EditorNodeTypeData> GetNodeTypes()
        {
            LoadConfig();
            List<EditorNodeTypeData> EnumDatas = new List<EditorNodeTypeData>();
            List<EditorNetNodeTypeData> EnumNetDatas = new List<EditorNetNodeTypeData>();
            var values = Enum.GetValues(typeof(ProcessNodeType));
            foreach (var value in values)
            {
                var attribute = value.GetType().GetField(value.ToString()).GetCustomAttribute<InspectorNameAttribute>();
                var name = Enum.GetName(typeof(ProcessNodeType), value);
                var type = value.GetHashCode();
                EditorNodeTypeData data = new EditorNodeTypeData();
                data.name = name;
                data.value = type;
                data.desc = attribute?.displayName;
                data.gourp = NodeGroups[type].Group;
                data.type = (ProcessNodeType)value.GetHashCode();
                EnumDatas.Add(data);
            }

            return new List<EditorNodeTypeData>(EnumDatas);
        }

        public static void AddNodeType(string name, string desc, string gourpName, bool client = true)
        {
            var clientTypes = GetNodeTypes();

            int maxIndex = 0;
            if (client)
            {
                foreach (var type in clientTypes)
                {
                    if (type.type == ProcessNodeType.End) 
                        continue;

                    if (type.value > maxIndex)
                    {
                        maxIndex = type.value;
                    }
                }
                
                EditorNodeTypeData data = new EditorNodeTypeData();
                data.name = name;
                data.value = maxIndex + 1;
                data.desc = desc;
                data.gourp = gourpName;
                clientTypes.Add(data);
                clientTypes.Sort((x, y) => x.value.CompareTo(y.value));
            }
            WriteNodeGroupConfig(clientTypes);
            WriteType(clientTypes);
        }
        

        public static void WriteType(List<EditorNodeTypeData> client)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("");
            builder.AppendLine("namespace Process.Common");
            builder.AppendLine("{");
            builder.AppendLine("    // 注意：自动生成代码，请勿手动修改");
            builder.AppendLine("    [Serializable]");
            builder.AppendLine("    public enum ProcessNodeType");
            builder.AppendLine("    {");
            //客户端节点
            builder.AppendLine("        #region 客户端节点");
            builder.AppendLine("");
            foreach (var type in client)
            {
                builder.AppendLine($"        [InspectorName(\"{type.desc}\")]");
                builder.AppendLine($"        {type.name} = {type.value},");
                builder.AppendLine("");
            }
            builder.AppendLine("");
            builder.AppendLine("        #endregion");
            builder.AppendLine("");
            builder.AppendLine("    }");
            builder.AppendLine("}");
            
            ProcessWriter.WriteFile(builder, GlobalPathConfig.ProcessNodeTypePath);
        }
        
        public static void WriteNodeGroupConfig(List<EditorNodeTypeData> client)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("{\"NodeConfig\":[");
            for (int i = 0; i < client.Count; i++)
            {
                var nodeType = client[i];
                builder.Append($"   {{\"ID\":{nodeType.value},\"Group\":\"{nodeType.gourp}\",\"Name\":\"{nodeType.desc}\"}}");
                if (i != client.Count - 1) builder.Append(",");
                builder.AppendLine("");
            }
            builder.AppendLine("]}");
            ProcessWriter.WriteFile(builder, GlobalPathConfig.NodeGroupConfigPath);
        }
    }
}