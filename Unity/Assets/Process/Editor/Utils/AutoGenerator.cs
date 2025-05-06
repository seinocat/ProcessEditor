using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphProcessor;
using UnityEditor;
using UnityEngine;
using Process.Runtime;

namespace Process.Editor
{
    /// <summary>
    /// 自动化生成工具
    /// </summary>
    public static class AutoGenerator
    {
        /// <summary>
        /// 一键生成
        /// </summary>
        [MenuItem("Tools/ProcessEditor/一键生成")]
        public static void OneKeyGenerate()
        {
            GenerateDataNodeWriter();
            GenerateDataNodeReader();
            GenerateRuntimeNode();
            // GenerateProcessNodePool();
            // GenerateLogicNode();
        }
        
        /// <summary>
        /// 自动生成DataNodeWriter，用于序列化数据
        /// </summary>
        [MenuItem("Tools/ProcessEditor/GenerateDataNodeWriter")]
        public static void GenerateDataNodeWriter()
        {
            //先找到所有继承自ProcessNodeBase的类
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(EditorEditorNode)) && t.IsSubclassOf(typeof(ProcessEditorNodeBase)))
                .ToList();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeWriter ***/");
            builder.AppendLine("using System.IO;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("using Seino.Utils.FastFileReader;");
            builder.AppendLine("");
            builder.AppendLine("namespace Process.Editor");
            builder.AppendLine("{");
            
            foreach (var type in types)
            {
                //获取所有带CustomSetting标签且需要导出的字段
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p =>
                    {
                        var customAttr = p.GetCustomAttribute(typeof(CustomSettingAttribute), false);
                        if (customAttr is CustomSettingAttribute customSettingAttribute)
                            return customSettingAttribute.export;
                        return false;
                    })
                    .ToList();

                if (fields.Count == 0)
                    continue;
                
                builder.AppendLine($"    public class {type.Name}Writer : IFileWriter");
                builder.AppendLine("    {");
                builder.AppendLine($"        public {type.Name} Data;");
                builder.AppendLine("        public float Progress => 0;");
                builder.AppendLine("        public bool IsComplete { get; }");
                builder.AppendLine("");
                builder.AppendLine("        public Task WriteAsync(BinaryWriter writer)");
                builder.AppendLine("        {");

                foreach (var field in fields)
                {
                    builder.AppendLine($"            writer.Write(Data.{field.Name});");
                }

                builder.AppendLine("            return Task.CompletedTask;");
                builder.AppendLine("        }");
                builder.AppendLine("    }");
                builder.AppendLine("");
            }
            
            builder.AppendLine("}");
            
            ProcessWriter.WriteFile(builder, GlobalPathConfig.NodeDataWriterPath);
        }
        
        /// <summary>
        /// 自动生成DataNodeReader，用于反序列化数据
        /// </summary>
        [MenuItem("Tools/ProcessEditor/GenerateDataNodeReader")]
        public static void GenerateDataNodeReader()
        {
            //先找到所有继承自ProcessNodeBase的类
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(EditorEditorNode)) && t.IsSubclassOf(typeof(ProcessEditorNodeBase)))
                .ToList();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeReader ***/");
            builder.AppendLine("using System.IO;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("using Seino.Utils.FastFileReader;");
            builder.AppendLine("");
            builder.AppendLine("namespace Process.Runtime");
            builder.AppendLine("{");
            
            foreach (var type in types)
            {
                //获取所有带CustomSetting标签且需要导出的字段
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p =>
                    {
                        var customAttr = p.GetCustomAttribute(typeof(CustomSettingAttribute), false);
                        if (customAttr is CustomSettingAttribute customSettingAttribute)
                            return customSettingAttribute.export;
                        return false;
                    })
                    .ToList();

                if (fields.Count == 0)
                    continue;
                string className = type.Name.Replace("EditorNode", "Node");
                builder.AppendLine($"    public class {className}Reader : IFileReader");
                builder.AppendLine("    {");
                builder.AppendLine("        public float Progress => 0;");
                builder.AppendLine("        public bool IsComplete { get; }");
                builder.AppendLine("");
                builder.AppendLine("        public Task ReadAsync(BinaryReader reader)");
                builder.AppendLine("        {");

                foreach (var field in fields)
                {
                    builder.AppendLine($"            reader.Read{field.FieldType.Name}();");
                }

                builder.AppendLine("            return Task.CompletedTask;");
                builder.AppendLine("        }");
                builder.AppendLine("    }");
                builder.AppendLine("");
            }
            
            builder.AppendLine("}");
            
            ProcessWriter.WriteFile(builder, GlobalPathConfig.NodeDataReaderPath);
        }

        /// <summary>
        /// 自动生成RuntimeNode，用于运行时使用
        /// </summary>
        [MenuItem("Tools/ProcessEditor/GenerateRuntimeNode")]
        public static void GenerateRuntimeNode()
        {
            //先找到所有继承自ProcessNodeBase的类
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(EditorEditorNode)) && t.IsSubclassOf(typeof(ProcessEditorNodeBase)))
                .ToList();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateRuntimeNode ***/");
            builder.AppendLine("using System;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("");
            builder.AppendLine("namespace Process.Runtime");
            builder.AppendLine("{");
            
            foreach (var type in types)
            {
                //获取所有带CustomSetting标签且需要导出的字段
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p =>
                    {
                        var customAttr = p.GetCustomAttribute(typeof(CustomSettingAttribute), false);
                        if (customAttr is CustomSettingAttribute customSettingAttribute)
                            return customSettingAttribute.export;
                        return false;
                    })
                    .ToList();
                
                //先统计一下类型数量
                Dictionary<Type, int> typeCount = new Dictionary<Type, int>();
                
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    typeCount.TryAdd(fieldType.IsEnum ? typeof(Enum) : fieldType, 0);
                }
                
                string className = type.Name.Replace("EditorNode", "Node");
                string isStart = className.Equals("StartNode") ? "true" : "false";
                builder.AppendLine($"    public partial class {className} : ProcessNodeBase");
                builder.AppendLine("    {");
                builder.AppendLine($"         public override ProcessNodeType Type => ProcessNodeType.{type.Name.Replace("EditorNode", string.Empty)};");
                builder.AppendLine($"         public override bool IsStart => {isStart};");
                builder.AppendLine("");
                
                //字段声明
                foreach (var field in fields)
                {
                    builder.AppendLine($"         private {GetFieldAlias(field)} m_{field.Name};");
                }
                
                builder.AppendLine("");
                builder.AppendLine("         protected override void ReadNodeData()");
                builder.AppendLine("         {");
                
                //写入字段数据
                foreach (var field in fields)
                {
                    var isEnum = field.FieldType.IsEnum;
                    var fieldName = GetFieldName(field, ++typeCount[isEnum ? typeof(Enum) : field.FieldType]);
                    //枚举类型需要转换
                    builder.AppendLine(isEnum
                        ? $"             m_{field.Name} = ({field.FieldType.Name})config.{fieldName};"
                        : $"             m_{field.Name} = config.{fieldName};");
                }
                
                builder.AppendLine("         }");
                builder.AppendLine("");
                builder.AppendLine("         protected override void ClearNodeData()");
                builder.AppendLine("         {");
                
                //清理数据
                foreach (var field in fields)
                {
                    builder.AppendLine($"             m_{field.Name} = {GetFieldDefaultValue(field)};");
                }
                
                builder.AppendLine("         }");
                builder.AppendLine("");
                
                //回收节点
                builder.AppendLine("         public override void Recycle()");
                builder.AppendLine("         {");
                builder.AppendLine($"             NodePool<{className}>.Recycle(this);");
                builder.AppendLine("         }");
                
                builder.AppendLine("");
                builder.AppendLine("    }");
                builder.AppendLine("");
            }
            
            builder.AppendLine("}");
            
            ProcessWriter.WriteFile(builder, GlobalPathConfig.RunTimeNodePath);
        }

        /// <summary>
        /// 自动生成LogicNode，用于逻辑处理
        /// </summary>
        // [MenuItem("Tools/ProcessEditor/GenerateLogicNode")]
        [Obsolete("会覆盖原有的逻辑处理节点")]
        public static void GenerateLogicNode()
        {
            //先找到所有继承自ProcessNodeBase的类
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(EditorEditorNode)) && t.IsSubclassOf(typeof(ProcessEditorNodeBase)))
                .ToList();
            
            foreach (var type in types)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("using UnityEngine;");
                builder.AppendLine("");
                builder.AppendLine("/*** 工具生成 => Tools/ProcessEditor/GenerateLogicNode ***/");
                builder.AppendLine("namespace Process.Common");
                builder.AppendLine("{");
                
                //获取所有带CustomSetting标签且需要导出的字段
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p =>
                    {
                        var customAttr = p.GetCustomAttribute(typeof(CustomSettingAttribute), false);
                        if (customAttr is CustomSettingAttribute customSettingAttribute)
                            return customSettingAttribute.export;
                        return false;
                    })
                    .ToList();

                if (fields.Count == 0)
                    continue;
                string className = type.Name.Replace("EditorNode", "Node");
                builder.AppendLine($"    public partial class {className}  : ProcessNodeBase");
                builder.AppendLine("    {");
                builder.AppendLine("");
                builder.AppendLine("    }");
                builder.AppendLine("");
                builder.AppendLine("}");
                ProcessWriter.WriteFile(builder, $"{GlobalPathConfig.LogicNodePath}/{className}.cs");
            }
        }


        /// <summary>
        /// 不需要生成的运行时节点列表
        /// </summary>
        private static List<ProcessNodeType> m_BlackNodeList = new List<ProcessNodeType>()
        {
            // ProcessNodeType.WaitTime,
        };
        
        /// <summary>
        /// 生成节点池
        /// </summary>
        [MenuItem("Tools/ProcessEditor/GenerateProcessNodePool")]
        public static void GenerateProcessNodePool()
        {
            StringBuilder builder = new StringBuilder();
            
            var values = Enum.GetValues(typeof(ProcessNodeType));
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateProcessNodePool ***/");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("");
            builder.AppendLine("namespace Process.Common");
            builder.AppendLine("{");
            builder.AppendLine("    public static class ProcessNodePool");
            builder.AppendLine("    {");
            
            builder.AppendLine("        public static ProcessNodeBase Get(ProcessNodeType type)");
            builder.AppendLine("        {");
            builder.AppendLine("            switch (type)");
            builder.AppendLine("            {");
            foreach (ProcessNodeType value in values)
            {
                if (m_BlackNodeList.Contains(value))
                    continue;
                
                string typeName = Enum.GetName(typeof(ProcessNodeType), value);
                builder.AppendLine($"                case ProcessNodeType.{typeName}:");
                builder.AppendLine($"                    return NodePool<{typeName}Node>.Get();");
            }
            builder.AppendLine("                default:");
            builder.AppendLine("                    Debug.LogError($\"invalid process node type：{type}\");");
            builder.AppendLine("                    return null;");
            builder.AppendLine("            }");
            builder.AppendLine("        }");
            builder.AppendLine("");
            
            //释放所有NodePool
            builder.AppendLine("        public static void DisposeAllPools()");
            builder.AppendLine("        {");
            foreach (ProcessNodeType value in values)
            {
                if (m_BlackNodeList.Contains(value))
                    continue;
                
                string typeName = Enum.GetName(typeof(ProcessNodeType), value);
                builder.AppendLine($"            NodePool<{typeName}Node>.Dispose();");
            }
            builder.AppendLine("        }");
            
            builder.AppendLine("    }");
            builder.AppendLine("}");
            
            ProcessWriter.WriteFile(builder, GlobalPathConfig.NodePoolPath);
        }

        /// <summary>获取字段名称</summary>
        /// <param name="field">字段属性</param>
        /// <param name="index">字段索引</param>
        /// <returns></returns>
        private static string GetFieldName(FieldInfo field, int index)
        {
            //先判断是否是枚举
            if (field.FieldType.BaseType == typeof(Enum))
                return $"EnumValue{index}";
            
            //常规类型
            switch (field.FieldType.Name)
            {
                case "Int32":
                    return $"IntValue{index}";
                case "UInt32":
                    return $"UIntValue{index}";
                case "Int64":
                    return $"LongValue{index}";
                case "UInt64":
                    return $"ULongValue{index}";
                case "Single":
                    return $"FloatValue{index}";
                case "String":
                    return $"StringValue{index}";
                case "Boolean":
                    return $"BoolValue{index}";
                case "Vector2":
                    return $"Vector2Value{index}";
                case "Vector3":
                    return $"Vector3Value{index}";
                case "Quaternion":
                    return $"QuaternionValue{index}";
                case "Color":
                    return $"ColorValue{index}";
                default:
                    Debug.LogError($"无效的字段类型：{field.FieldType.Name}");
                    return $"ErrorValue{index}";
            }
        }
        
        /// <summary>
        /// 获取字段类型的别名
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetFieldAlias(FieldInfo field)
        {
            switch (field.FieldType.Name)
            {
                case "Int32":
                    return "int";
                case "UInt32":
                    return "uint";
                case "Int64":
                    return "long";
                case "UInt64":
                    return "ulong";
                case "Single":
                    return "float";
                case "String":
                    return "string";
                case "Boolean":
                    return "bool";
                case "Color":
                    return "Color";
                default:
                    return field.FieldType.Name;
            }
        }

        /// <summary>
        /// 获取字段默认值
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetFieldDefaultValue(FieldInfo field)
        {
            switch (field.FieldType.Name)
            {
                case "Int32":
                    return "0";
                case "UInt32":
                    return "0";
                case "Int64":
                    return "0";
                case "UInt64":
                    return "0";
                case "Single":
                    return "0f";
                case "String":
                    return "string.Empty";
                case "Boolean":
                    return "false";
                case "Vector2":
                    return "Vector2.zero";
                case "Vector3":
                    return "Vector3.zero";
                case "Quaternion":
                    return "Quaternion.identity";
                case "Color":
                    return "Color.black";
                default:
                    return "default";
            }
        }
    }
}