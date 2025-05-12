using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphProcessor;
using UnityEditor;
using Process.Runtime;

namespace Process.Editor
{
    /// <summary>
    /// 自动化生成工具
    /// </summary>
    public static class AutoGenerator
    {
        /// <summary>
        /// 不需要生成的运行时节点列表
        /// </summary>
        private static List<ProcessNodeType> m_BlackNodeList = new List<ProcessNodeType>()
        {
            // ProcessNodeType.WaitTime,
        };
        
        /// <summary>
        /// 一键生成
        /// </summary>
        [MenuItem("Tools/ProcessEditor/一键生成")]
        public static void OneKeyGenerate()
        {
            GenerateIOExtension();
            GenerateDataNodeWriter();
            GenerateDataNodeReader();
            GenerateRuntimeNode();
            GenerateProcessNodePool();
        }

        /// <summary>
        /// 生成自定义类型扩展
        /// </summary>
        // [MenuItem("Tools/ProcessEditor/GenerateIOExtension")]
        public static void GenerateIOExtension()
        {
            //先找到打了CustomData标签的类
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsClass && t.GetCustomAttribute<CustomDataAttribute>() != null)
                .ToList();
            
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateIOExtension ***/");
            builder.AppendLine("using System.IO;");
            builder.AppendLine("using Seino.Utils.FastFileReader;");
            builder.AppendLine("");
            builder.AppendLine("namespace Process.Runtime");
            builder.AppendLine("{");
            
            builder.AppendLine("    public static class GenerateIOExtension");
            builder.AppendLine("    {");
            foreach (var type in types)
            {
                //获取所有字段
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
                if (fields.Count == 0)
                    continue;
                
                // BinaryWriter
                builder.AppendLine($"        public static void Write(this BinaryWriter writer, {type.Name} value)");
                builder.AppendLine("        {");

                foreach (var field in fields)
                {
                    //枚举类型需要转换
                    if (field.FieldType.IsEnum)
                    {
                        builder.AppendLine($"            writer.Write((int)value.{field.Name});");
                        continue;
                    }

                    // List类型需要转换
                    if (typeof(IList).IsAssignableFrom(field.FieldType))
                    {
                        builder.AppendLine($"            writer.Write((int){field.Name}.Count);");
                        builder.AppendLine($"            foreach (var element in {field.Name})");
                        builder.AppendLine("            {");
                        builder.AppendLine("                   writer.Write(element);");
                        builder.AppendLine("            }");
                        continue;
                    }
                    
                    builder.AppendLine($"            writer.Write(value.{field.Name});");
                }
                
                builder.AppendLine("        }");
                builder.AppendLine("");
                
                // BinaryReader
                builder.AppendLine($"        public static {type.Name} Read{type.Name}(this BinaryReader reader)");
                builder.AppendLine("        {");
                builder.AppendLine($"            var value = new {type.Name}();");
                foreach (var field in fields)
                {
                    //枚举类型需要转换
                    if (field.FieldType.IsEnum)
                    {
                        builder.AppendLine($"            value.{field.Name} = ({field.FieldType.Name})reader.ReadInt32();");
                        continue;
                    }
                    
                    // List类型需要转换
                    if (typeof(IList).IsAssignableFrom(field.FieldType))
                    {
                        builder.AppendLine($"            var {field.Name}Count = reader.ReadInt32();");
                        builder.AppendLine($"            value.{field.Name} = new {field.FieldType.GetGenericArguments()[0].Name}[{field.Name}Count];");
                        builder.AppendLine($"            for(int i = 0; i < {field.Name}Count; i++)");
                        builder.AppendLine("            {");
                        builder.AppendLine($"                value.{field.Name}.Add(reader.Read{field.FieldType.GetGenericArguments()[0].Name}())");
                        builder.AppendLine("            }");
                        continue;
                    }
                    
                    builder.AppendLine($"            value.{field.Name} = reader.Read{field.FieldType.Name}();");
                }
                
                builder.AppendLine("            return value;");
                builder.AppendLine("        }");
                builder.AppendLine("");
            }
            
            builder.AppendLine("    }");
            builder.AppendLine("}");
            
            
            ProcessWriter.WriteFile(builder, GlobalPathConfig.IOExtensionPath);
        }
        
        /// <summary>
        /// 自动生成DataNodeWriter，用于序列化数据
        /// </summary>
        // [MenuItem("Tools/ProcessEditor/GenerateDataNodeWriter")]
        public static void GenerateDataNodeWriter()
        {
            //先找到所有继承自ProcessNodeBase的类
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(EditorEditorNode)) && t.IsSubclassOf(typeof(ProcessEditorNodeBase)))
                .ToList();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateDataNodeWriter ***/");
            builder.AppendLine("using System.IO;");
            builder.AppendLine("using Process.Runtime;");
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
                
                builder.AppendLine($"    public partial class {type.Name}");
                builder.AppendLine("    {");
                builder.AppendLine("        public override void WriteNodeData(BinaryWriter writer)");
                builder.AppendLine("        {");

                foreach (var field in fields)
                {
                    //枚举类型需要转换
                    if (field.FieldType.IsEnum)
                    {
                        builder.AppendLine($"            writer.Write((int){field.Name});");
                        continue;
                    }

                    // List类型需要转换
                    if (typeof(IList).IsAssignableFrom(field.FieldType))
                    {
                        builder.AppendLine($"            writer.Write({field.Name}.Count);");
                        builder.AppendLine($"            foreach (var element in {field.Name})");
                        builder.AppendLine("            {");
                        builder.AppendLine("                writer.Write(element);");
                        builder.AppendLine("            }");
                        continue;
                    }
                    
                    //枚举类型需要转换
                    builder.AppendLine($"            writer.Write({field.Name});");
                }
                
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
        // [MenuItem("Tools/ProcessEditor/GenerateDataNodeReader")]
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
            builder.AppendLine("using System.Collections.Generic;");
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
                
                string className = type.Name.Replace("EditorNode", "Node");
                builder.AppendLine($"    public class {className}Param : ProcessNodeParam");
                builder.AppendLine("    {");
                
                //字段声明
                foreach (var field in fields)
                {
                    builder.AppendLine($"        public {GetFieldAlias(field)} {field.Name};");
                }
                builder.AppendLine("");
                
                builder.AppendLine("        public override void ReadNodeData(BinaryReader reader)");
                builder.AppendLine("        {");
                foreach (var field in fields)
                {
                    //枚举类型需要转换
                    if (field.FieldType.IsEnum)
                    {
                        builder.AppendLine($"            {field.Name} = ({field.FieldType.Name})reader.ReadInt32();");
                        continue;
                    }
                    
                    // List类型需要转换
                    if (typeof(IList).IsAssignableFrom(field.FieldType))
                    {
                        builder.AppendLine("");
                        builder.AppendLine($"            {field.Name} = new {GetFieldAlias(field)}();");
                        builder.AppendLine($"            var {field.Name}Count = reader.ReadInt32();");
                        builder.AppendLine($"            for(int i = 0; i < {field.Name}Count; i++)");
                        builder.AppendLine("            {");
                        builder.AppendLine($"                {field.Name}.Add(reader.Read{field.FieldType.GetGenericArguments()[0].Name}());");
                        builder.AppendLine("            }");
                        builder.AppendLine("");
                        continue;
                    }
                    
                    builder.AppendLine($"            {field.Name} = reader.Read{field.FieldType.Name}();");
                    
                }
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
        // [MenuItem("Tools/ProcessEditor/GenerateRuntimeNode")]
        public static void GenerateRuntimeNode()
        {
            //先找到所有继承自ProcessNodeBase的类
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsSubclassOf(typeof(EditorEditorNode)) && t.IsSubclassOf(typeof(ProcessEditorNodeBase)))
                .ToList();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateRuntimeNode ***/");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using System.Collections.Generic;");
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
                builder.AppendLine("         public override void ReadNodeData(ProcessNodeParam data)");
                builder.AppendLine("         {");
                builder.AppendLine($"             if(data is {className}Param paramData)");
                builder.AppendLine("             {");
                
                //写入字段数据
                foreach (var field in fields)
                {
                    builder.AppendLine($"                 m_{field.Name} = paramData.{field.Name};");
                }
                
                builder.AppendLine("             }");
                
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
        /// 生成节点池
        /// </summary>
        // [MenuItem("Tools/ProcessEditor/GenerateProcessNodePool")]
        public static void GenerateProcessNodePool()
        {
            StringBuilder builder = new StringBuilder();
            
            var values = Enum.GetValues(typeof(ProcessNodeType));
            builder.AppendLine("/*** 工具自动生成 => Tools/ProcessEditor/GenerateProcessNodePool ***/");
            builder.AppendLine("using System;");
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("using System.Collections.Generic;");
            builder.AppendLine("");
            builder.AppendLine("namespace Process.Runtime");
            builder.AppendLine("{");
            builder.AppendLine("    public static class ProcessNodePool");
            builder.AppendLine("    {");
            builder.AppendLine("        private static readonly Dictionary<ProcessNodeType, Func<ProcessNodeBase>> m_FactoryMap = new();");
            builder.AppendLine("");
            
            builder.AppendLine("        static ProcessNodePool()");
            builder.AppendLine("        {");
            foreach (ProcessNodeType value in values)
            {
                if (m_BlackNodeList.Contains(value))
                    continue;
                
                string typeName = Enum.GetName(typeof(ProcessNodeType), value);
                builder.AppendLine($"             Register<{typeName}Node>(ProcessNodeType.{typeName});");
            }
            builder.AppendLine("        }");
            builder.AppendLine("");
            
            builder.AppendLine("        private static void Register<T>(ProcessNodeType type) where T : ProcessNodeBase, new()");
            builder.AppendLine("        {");
            builder.AppendLine("            m_FactoryMap[type] = () => NodePool<T>.Get();");
            builder.AppendLine("        }");
            builder.AppendLine("");
            
            builder.AppendLine("        public static ProcessNodeBase Get(ProcessNodeType type)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (m_FactoryMap.TryGetValue(type, out var creator))");
            builder.AppendLine("                return creator();");
            builder.AppendLine("");
            builder.AppendLine("            Debug.LogError($\"Invalid process node type: {type}\");");
            builder.AppendLine("            return null;");
            builder.AppendLine("        }");
            builder.AppendLine("");
            
            //释放所有NodePool
            builder.AppendLine("        public static void DisposeAllPools()");
            builder.AppendLine("        {");
            builder.AppendLine("             foreach (var pairs in m_FactoryMap)");
            builder.AppendLine("             {");
            builder.AppendLine("                 pairs.Value().Dispose();");
            builder.AppendLine("             }");
            builder.AppendLine("        }");
            
            builder.AppendLine("    }");
            builder.AppendLine("");
            builder.AppendLine("    public static class ProcessNodeParamCreator");
            builder.AppendLine("    {");
            builder.AppendLine("");
            builder.AppendLine("        private static readonly Dictionary<ProcessNodeType, Func<ProcessNodeParam>> m_FactoryMap = new();");
            builder.AppendLine("");
            
            builder.AppendLine("        static ProcessNodeParamCreator()");
            builder.AppendLine("        {");
            foreach (ProcessNodeType value in values)
            {
                if (m_BlackNodeList.Contains(value))
                    continue;
                
                string typeName = Enum.GetName(typeof(ProcessNodeType), value);
                builder.AppendLine($"             Register<{typeName}NodeParam>(ProcessNodeType.{typeName});");
            }
            builder.AppendLine("        }");
            builder.AppendLine("");
            
            builder.AppendLine("        private static void Register<T>(ProcessNodeType type) where T : ProcessNodeParam, new()");
            builder.AppendLine("        {");
            builder.AppendLine("            m_FactoryMap[type] = () => new T();");
            builder.AppendLine("        }");
            builder.AppendLine("");
            
            builder.AppendLine("        public static ProcessNodeParam Get(ProcessNodeType type)");
            builder.AppendLine("        {");
            builder.AppendLine("            if (m_FactoryMap.TryGetValue(type, out var creator))");
            builder.AppendLine("                return creator();");
            builder.AppendLine("");
            builder.AppendLine("            Debug.LogError($\"Invalid process node type: {type}\");");
            builder.AppendLine("            return null;");
            builder.AppendLine("        }");
            builder.AppendLine("");
            
            builder.AppendLine("    }");
            
            builder.AppendLine("}");
            
            ProcessWriter.WriteFile(builder, GlobalPathConfig.NodePoolPath);
        }
        
        /// <summary>
        /// 获取字段类型的别名
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetFieldAlias(FieldInfo field)
        {
            //列表类型
            if (typeof(IList).IsAssignableFrom(field.FieldType))
            {
                var elementType = field.FieldType.GetGenericArguments()[0];
                return $"List<{GetFieldAliasName(elementType.Name)}>";
            }
            
            return GetFieldAliasName(field.FieldType.Name);
        }

        /// <summary>
        /// 获取字段类型的别名名称
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static string GetFieldAliasName(string typeName)
        {
            //基础类型
            switch (typeName)
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
                    return typeName;
            }
        }

        /// <summary>
        /// 获取字段默认值
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private static string GetFieldDefaultValue(FieldInfo field)
        {
            if (typeof(IList).IsAssignableFrom(field.FieldType))
            {
                return "null";
            }
            
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