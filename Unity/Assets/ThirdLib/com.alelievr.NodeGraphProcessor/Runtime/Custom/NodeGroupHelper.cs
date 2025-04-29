using System.Collections.Generic;
using UnityEngine;

namespace GraphProcessor
{
    public static class NodeGroupHelper
    {
        public static Dictionary<int, NodeGroupData> NodeGroups;
        public static string ConfigPath => "Assets/Process/Editor/Resource/Config/NodeGroupConfig.json";

#if UNITY_EDITOR
        public static void LoadConfig()
        {
            NodeGroups = new Dictionary<int, NodeGroupData>();
            var config = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(ConfigPath)?.text;
            NodeGroup group = JsonUtility.FromJson<NodeGroup>(config);
            foreach (var data in group.NodeConfig)
                NodeGroups.Add(data.ID, data);
        }
#endif
        
        public static string GetMenuTitle(int id) => $"{GetGroup(id)}/{GetName(id)}";
        
        public static string GetGroup(int id)
        {
            if (NodeGroups == null) return string.Empty;

            if (!NodeGroups.TryGetValue(id, out var group))
            {
                Debug.LogError($"找不到对应ID={id}的配置，于NodeGroupConfig.json添加！");
                return "null";
            }
            
            return $"{group.Group}";
        }

        public static string GetName(int id)
        {
            if (NodeGroups == null) return string.Empty;
            
            if (!NodeGroups.TryGetValue(id, out var group))
            {
                Debug.LogError($"找不到对应ID={id}的配置，于NodeGroupConfig.json添加！");
                return "null";
            }
            return group.Name;
        }
    }
}