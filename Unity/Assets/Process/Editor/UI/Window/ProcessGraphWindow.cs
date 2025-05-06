using System.Collections.Generic;
using Process.Runtime;
using GraphProcessor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Process.Editor
{
    public class ProcessGraphWindow : BaseGraphWindow
    {
        public ProcessGraphBase Graph => graph as ProcessGraphBase;
        public ProcessGraphView GraphView => graphView as ProcessGraphView;
        
        public static string OPEN_GRAPH = "Process_CurOpen";
        
        public void SetGraph(BaseGraph baseGraph)
        {
            graph = baseGraph;
        }
        
        [MenuItem("Tools/ProcessEditor/Editor %#z")]
        public static void OpenWindow()
        {
            var window = GetWindow<ProcessGraphWindow>("ProcessEditor");
            var GraphBases = ProcessUtils.GetAllProcess();
            if (GraphBases.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "当前没有流程配置!请通过右键Create/Process创建！", "确定");
                return;
            }

            var open = Cookie.GetPublic(OPEN_GRAPH, string.Empty);
            ProcessGraphBase graph;
            if (string.IsNullOrEmpty(open))
            {
                graph = GraphBases[0];
            }
            else
            {
                graph = GraphBases.Find(x => x?.name == open);
                if (graph == null) graph = GraphBases[0];
            }
            
            window.SetGraph(graph);
            window.Show();
        }
        
        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var graph = EditorUtility.InstanceIDToObject(instanceID) as ProcessGraphBase;
            if (graph)
            {
                var window = GetWindow<ProcessGraphWindow>();
                Cookie.SetPublic(OPEN_GRAPH, graph.name);
                window.InitializeGraph(graph);
                window.Show();
            }
            return graph;
        }
        
        [MenuItem("Assets/Create/Process", false, 10)]
        public static void CreateGraphAsset()
        {
            var graph = CreateInstance<ProcessGraphBase>();
            var path = $"{GlobalPathConfig.GraphsAssetsPath}/New Process{ProcessUtils.GetProcessCount() + 1}.asset";
            AssetDatabase.CreateAsset(graph, path);
            AssetDatabase.Refresh();
        }
        
        protected override void InitializeWindow(BaseGraph baseGraph)
        {
            NodeGroupHelper.LoadConfig();
            if (baseGraph != null && (baseGraph.nodes == null || baseGraph.nodes.Count == 0))
            {
                var rootNode = BaseNode.CreateFromType(typeof(RootEditorNode), new Vector2(656.5f, 390.4f));
                baseGraph.AddNode(rootNode);
            }
            graphView = new ProcessGraphView(this);
            graphView.Add(new ProcessToolBarView(graphView));
            if (this.Graph != null) this.Graph.ProcessWindow = this;
            rootView.Add(graphView);
        }
    }
}