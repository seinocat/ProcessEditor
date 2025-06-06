﻿using Process.Runtime;
using UnityEngine;

namespace Process.Editor
{
    public static class GlobalPathConfig
    {
        //编辑器根目录
        public const string RootPath                    = "Assets/Process";
        //流程图资源路径
        public const string GraphsAssetsPath            = RootPath + "/Editor/ProcessGraphs";
        //流程图资源相对路径
        public const string GraphsRelativePath          = "Process/Editor/ProcessGraphs";
        //节点分组配置路径
        public const string ResIconPath                 = RootPath +  "/Editor/Resource/Icon";
        //编辑器逻辑节点路径
        public static string EditorNodePath             => Application.dataPath + "/Process/Editor/Node/Logic";
        //运行时节点类型路径
        public static string ProcessNodeTypePath        => Application.dataPath + "/Process/Runtime/Common/Enum/ProcessNodeType.cs";
        //节点数据写入方法路径(Editor)
        public static string NodeDataWriterPath         => Application.dataPath + "/Process/Editor/Generate/NodeDataWriter.cs";
        //IO扩展方法路径
        public static string IOExtensionPath            => Application.dataPath + "/Process/Runtime/Generate/IOExtension.cs";
        //节点数据读取方法路径(Runtime)
        public static string NodeDataReaderPath         => Application.dataPath + "/Process/Runtime/Generate/NodeDataReader.cs";
        //运行时节点生成路径
        public static string RunTimeNodePath            => Application.dataPath + "/Process/Runtime/Generate/RunTimeNode.cs";
        //逻辑节点生成路径
        public static string LogicNodePath              => Application.dataPath + "/Process/Runtime/Common/Node";
        //运行时节点转换路径
        public static string NodePoolPath               => Application.dataPath + "/Process/Runtime/Generate/ProcessNodePool.cs";
        //节点分组配置路径
        public static string NodeGroupConfigPath        => Application.dataPath + "/Process/Editor/Resource/Config/NodeGroupConfig.json";
        // 节点分组配置资源路径
        public static string NodeGroupConfigAssetPath   => "Assets/Process/Editor/Resource/Config/NodeGroupConfig.json";
    }
}