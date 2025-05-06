using UnityEditor;
using UnityEngine;

namespace Process.Editor
{
    public class GraphItemData
    {
        public string Name;
        public string Path;
        public string ProcessPath;
        public bool IsFolder;
        
        public string DiskPath      => Application.dataPath.Replace("Assets", Path);
        public string EventDiskPath = $"{GlobalPathConfig.GraphsAssetsPath}/";
        
        public static GraphItemData CreateDictory(string path)
        {
            GraphItemData data = new GraphItemData();
            data.IsFolder = true;
            data.Path = path;
            data.ProcessPath = data.Path[data.EventDiskPath.Length..];
            data.Name = System.IO.Path.GetFileName(data.Path);
            return data;
        }
        
        public static GraphItemData CreateGraph(ProcessGraphBase graph)
        {
            GraphItemData data = new GraphItemData();
            data.IsFolder = false;
            data.Path = AssetDatabase.GetAssetPath(graph);
            data.ProcessPath = data.Path[data.EventDiskPath.Length..];
            data.Name = graph.name;
            return data;
        }
    }
}