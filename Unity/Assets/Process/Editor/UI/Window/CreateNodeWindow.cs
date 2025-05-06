using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Process.Runtime;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Process.Editor
{
    public class CreateNodeWindow : OdinEditorWindow
    {
        [Serializable]
        public enum NodeType
        {
            [LabelText("客户端节点")]
            Client,
            [LabelText("服务器节点")]
            Server,
            [LabelText("Editor节点")]
            Editor,
        }

        [Serializable]
        public enum FieldType
        {
            [LabelText("Int")] 
            IntType,
            
            [LabelText("String")] 
            StringType,
            
            [LabelText("Vector3")] 
            Vector3Type,
            
            [LabelText("Float")]
            FloatType,
            
            [LabelText("Bool")]
            BoolType,
            
            [LabelText("Ulong")]
            UlongType,
            
            [LabelText("Object")] 
            ObjectType,
            
            [LabelText("List")] 
            ListType,
            
            [LabelText("Custom")] 
            CustomType,
        }

        [Serializable]
        public class FieldData
        {
            [LabelText("类型")] 
            public FieldType fieldType;

            [LabelText("List类型"), ShowIf("fieldType", FieldType.ListType)]
            public FieldType listType;

            [LabelText("自定义类型名"), ShowIf("@fieldType == FieldType.CustomType || listType == FieldType.CustomType")]
            public string typeName;

            [LabelText("字段名")] 
            public string fieldName;
            
            [LabelText("备注")] 
            public string desc;
        }

        [MenuItem("Tools/ProcessEditor/CreateNode %#x")]
        public static void Open()
        {
            var window = GetWindow<CreateNodeWindow>();
            window.Show();
        }

        [LabelText("类型"), HideInInspector] 
        public NodeType nodeType = NodeType.Client;

        [LabelText("枚举名称")] 
        public string enumName;
        
        [LabelText("节点名称")] 
        public string nodeName;

        [LabelText("节点组名")]
        public string groupName;
        
        [LabelText("字段")] 
        public List<FieldData> fieldList = new List<FieldData>();

        [Button("创建节点")]
        public void Create()
        {
            if (!Check()) 
                return;
            
            WriteEditorNode();
            WriteNodeType();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("提示", "创建成功!", "确定");
            Close();
        }
        
        private bool Check()
        {
            var typeNames = Enum.GetNames(typeof(ProcessNodeType)).ToList();
            if (typeNames.Contains(this.enumName))
            {
                EditorUtility.DisplayDialog("提示", $"{this.enumName}节点已经存在!", "确定");
                return false;
            }
            return true;
        }

        public void WriteEditorNode()
        {
            StringBuilder builder = new StringBuilder();
            bool hasList = fieldList.Exists(x => x.fieldType == FieldType.ListType);
            
            builder.AppendLine("using System;");
            builder.AppendLine("using GraphProcessor;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using Process.Common;");
            
            if (hasList) builder.AppendLine("using System.Collections.Generic;");

            builder.AppendLine("");
            builder.AppendLine("namespace Process.Editor");
            builder.AppendLine("{");
            builder.AppendLine($"    [NodeMenuItem((int)ProcessNodeType.{enumName}), ProcessNode, Serializable]");
            builder.AppendLine($"    public partial class {enumName}EditorNode : CommonEditorNode");
            builder.AppendLine("    {");
            if (nodeType != NodeType.Editor)
            {
                builder.AppendLine($"        public override ProcessNodeType Type => ProcessNodeType.{enumName};");
            }
            foreach (var field in fieldList)
            {
                builder.AppendLine("");
                
                if (field.fieldType == FieldType.ListType)
                {
                    if (field.listType == FieldType.CustomType)
                    {
                        builder.AppendLine($"        [CustomSetting(\"{field.desc}\"), ListReference(typeof({field.typeName}), nameof({field.fieldName}))] ");
                        builder.AppendLine($"        public List<{field.typeName}> {field.fieldName};");
                    }
                    else
                    {
                        builder.AppendLine($"        [CustomSetting(\"{field.desc}\"), ListReference(typeof({GetTypeName(field.listType)}), nameof({field.fieldName}))] ");
                        builder.AppendLine($"        public List<{GetTypeName(field.listType)}> {field.fieldName};");
                    }
                }
                else if(field.fieldType == FieldType.CustomType)
                {
                    builder.AppendLine($"        [CustomSetting(\"{field.desc}\")] ");
                    builder.AppendLine($"        public {field.typeName} {field.fieldName};");
                }
                else
                {
                    builder.AppendLine($"        [CustomSetting(\"{field.desc}\")] ");
                    builder.AppendLine($"        public {GetTypeName(field.fieldType)} {field.fieldName};");
                }

            }
            
            builder.AppendLine("    }");
            builder.AppendLine("}");
            
            Save(builder);
        }
        
        public void WriteNodeType()
        {
            if (nodeType != NodeType.Editor)
            {
                NodeTypeUtils.AddNodeType(enumName, nodeName, groupName, nodeType == NodeType.Client);
            }
        }

        public string GetTypeName(FieldType type)
        {
            string typeName;
            switch (type)
            {
                case FieldType.IntType:
                    return "int";
                case FieldType.StringType:
                    return "string";
                case FieldType.Vector3Type:
                    return "Vector3";
                case FieldType.ObjectType:
                    return "Object";
                case FieldType.ListType:
                    return "List";
                case FieldType.FloatType:
                    return "float";
                case FieldType.BoolType:
                    return "bool";
                case FieldType.UlongType:
                    return "ulong";
                default:
                    return "int";
            }
        }

        public void Save(StringBuilder builder)
        {
            string path = $"{GlobalPathConfig.EditorNodePath}/{enumName}EditorNode.cs";
            FileStream fileStream = new FileStream(path, FileMode.Create);
            StreamWriter fileWriter = new StreamWriter(fileStream, Encoding.UTF8);
            try
            {
                fileWriter.Write(builder.ToString());
                fileWriter.Flush();
            }
            catch (Exception ex)
            {
                Debug.LogError($"生成失败 :{ex}");
            }
            finally
            {
                fileWriter.Close();
                fileStream.Close();
            }
            
            
        }
        
    }
}