using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GraphProcessor;
using UnityEditor;

namespace Process.Editor
{
    public static class ProcessUtils
    {
        private static List<ProcessGraphBase> m_ProcessGraphsCache;

        /// <summary>
        /// 获取所有流程配置
        /// </summary>
        /// <returns></returns>
        public static List<ProcessGraphBase> GetAllProcess()
        {
            return m_ProcessGraphsCache ??= LoadAllAssets<ProcessGraphBase>(GlobalPathConfig.GraphsAssetsPath);
        }
        
        /// <summary>
        /// 获取所有流程配置数量
        /// </summary>
        /// <returns></returns>
        public static int GetProcessCount()
        {
            return GetAllProcess().Count;
        }
        
        /// <summary>
        /// 获取路径下所有指定类型的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="option"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> LoadAllAssets<T>(string path, SearchOption option = SearchOption.AllDirectories) where T : UnityEngine.Object
        {
            List<T> list = new List<T>();
            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles("*", option);

                foreach (var file in files)
                {
                    if (file.Name.EndsWith(".meta")) continue;

                    string assetName = file.FullName;
                    string assetPath = assetName[assetName.IndexOf("Assets", StringComparison.Ordinal)..];
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset)
                    {
                        list.Add(asset);
                    }
                }
            }
            
            return list;
        }

        public static List<string> GetSubFolders(string path, bool onlyName = false)
        {
            var folders = AssetDatabase.GetSubFolders(path).ToList();
            if (onlyName)
            {
                List<string> subFloders = new List<string>();
                for (int i = 0; i < folders.Count; i++)
                {
                    subFloders.Add(Path.GetFileName(folders[i]));
                }
                return subFloders;
            }

            return folders;
        }
        
        public static List<string> GetParentFolders(string path)
        {
            var folderPath = path.Split('/').ToList();
            List<string> folders = new List<string>();
            var count = folderPath.Count;
            for (int i = folderPath.Count - 1; i >= 0; i--)
            {
                if (i == count - 1)
                {
                    folderPath.RemoveAt(i);
                    continue;
                }
                
                StringBuilder resultBuilder = new StringBuilder();
                // 遍历字符数组
                for (int j = 0; j < folderPath.Count; j++)
                {
                    // 将字符添加到 StringBuilder
                    resultBuilder.Append(folderPath[j]);
                    // 如果不是最后一个字符，添加斜杠
                    if (j < folderPath.Count - 1)
                    {
                        resultBuilder.Append('/');
                    }
                }
                folderPath.RemoveAt(i);
                folders.Add(resultBuilder.ToString());
            }
            return folders;
        }

        public static List<BaseNode> GetOutputNodeList(this BaseNode node)
        {
            List<BaseNode> nodeList = new List<BaseNode>();
            var nodes = node.GetOutputNodes();
            foreach (var item in nodes)
                nodeList.Add(item);
            return nodeList;
        }
        
        public static List<BaseNode> GetInputNodeList(this BaseNode node)
        {
            List<BaseNode> nodeList = new List<BaseNode>();
            var nodes = node.GetInputNodes();
            foreach (var item in nodes)
            {
                nodeList.Add(item);
            }
            return nodeList;
        }
    }
}