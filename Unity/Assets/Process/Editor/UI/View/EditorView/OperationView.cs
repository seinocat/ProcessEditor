using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Process.Editor
{
    public sealed class OperationView : Blackboard
    {
        private ProcessGraphView    m_GraphView;
        private ProcessGraphWindow  m_Window;
        private ProcessGraphBase    m_Graph;
        private const string WIDTH  = "ProcessEditor_Operation_Width";
        private const string HEIGHT = "ProcessEditor_Operation_Heigh";
        private const string POSX   = "ProcessEditor_Operation_PosX";
        private const string POSY   = "ProcessEditor_Operation_PosY";
    
        public OperationView(ProcessGraphView graphView)
        {
            m_Window = graphView.Window;
            m_GraphView = graphView;
            scrollable = true;
            addItemRequested = OnAddBtnClick;
            m_Graph = m_Window.Graph;
            SetPosition(new Rect(Cookie.GetPublic(POSX, 0f), Cookie.GetPublic(POSY, 0f), Cookie.GetPublic(WIDTH, 340f), Cookie.GetPublic(HEIGHT, 200f)));
            DrawView();
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        public void DrawView()
        {
            if (m_Graph == null)
            {
                return;
            }

            float percentWidth = 85f;
            StyleLength length = new StyleLength(new Length(percentWidth, LengthUnit.Percent));
            
            Label graphName = new Label();
            graphName.text = m_Graph.name;
            graphName.style.alignSelf = Align.Center;
            Add(graphName);

            Button exportBtn = new Button(ExportGraph);
            exportBtn.text = "导出当前";
            exportBtn.style.width = length;
            exportBtn.style.alignSelf = Align.Center;
            Add(exportBtn);
            
            Button exportAllBtn = new Button(ExportAll);
            exportAllBtn.text = "导出全部";
            exportAllBtn.style.width = length;
            exportAllBtn.style.alignSelf = Align.Center;
            Add(exportAllBtn);
            
            Button exportServerBtn = new Button(ExportServer);
            exportServerBtn.text = "导出服务器配置";
            exportServerBtn.style.width = length;
            exportServerBtn.style.alignSelf = Align.Center;
            Add(exportServerBtn);
            
            Button oneKeyExportBtn = new Button(OneKeyExport);
            oneKeyExportBtn.text = "一键导出所有配置";
            oneKeyExportBtn.style.width = length;
            oneKeyExportBtn.style.alignSelf = Align.Center;
            Add(oneKeyExportBtn);
            
            Button jsonPathBtn = new Button(OpenJsonConfigPath);
            jsonPathBtn.text = "打开配置目录";
            jsonPathBtn.style.width = length;
            jsonPathBtn.style.alignSelf = Align.Center;
            Add(jsonPathBtn);
            
            Button delBtn = new Button(OnDelClick);
            delBtn.text = "删除";
            delBtn.style.width = length;
            delBtn.style.alignSelf = Align.Center;
            Add(delBtn);
        }
        
        public void OnAddBtnClick(Blackboard blackboard)
        {
            CustomGameEventWindow.OpenWindow("创建新流程", "New Process", (CreateName) =>
            {
                var graph = ScriptableObject.CreateInstance<ProcessGraphBase>();
                var path = $"Assets/Editor/ProcessGraphs/{CreateName}.asset";
                if (!m_GraphView.FileView.IsGraphExist(CreateName))
                {
                    AssetDatabase.CreateAsset(graph, path);
                    AssetDatabase.Refresh();
                    m_GraphView.FileView.Repaint();
                    CustomGameEventWindow.CloseWindow();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该配置已存在!", "确定");
                }
                
            });
        }

        public void ExportGraph()
        {
            // if (ProcessExportUtils.ExportGraph(this.m_Graph))
            // {
            //     EditorUtility.DisplayDialog("提示", $"导出{this.m_Graph.name}成功", "确定");
            // }
            // else
            // {
            //     EditorUtility.DisplayDialog("提示", $"导出{this.m_Graph.name}失败，请根据报错日志修改！", "确定");
            // }
        }

        public void ExportAll()
        {
            // if (ProcessExportUtils.ExportAllGraph())
            // {
            //     EditorUtility.DisplayDialog("提示", "全部导出成功", "确定");
            // }
            // else
            // {
            //     EditorUtility.DisplayDialog("提示", "导出失败，请根据报错日志修改！", "确定");
            // }
        }

        public void ExportServer()
        {
            // if (ProcessExportUtils.ExportServer())
            // {
            //     EditorUtility.DisplayDialog("提示", "导出服务器配置成功", "确定");
            // }
            // else
            // {
            //     EditorUtility.DisplayDialog("提示", "导出失败，请根据报错日志修改！", "确定");
            // }
        }

        public void OneKeyExport()
        {
            // if (ProcessExportUtils.ExportAllGraph() && ProcessExportUtils.ExportServer())
            // {
            //     EditorUtility.DisplayDialog("提示", "导出客户端和服务器配置成功", "确定");
            // }
            // else
            // {
            //     EditorUtility.DisplayDialog("提示", "导出失败，请根据报错日志修改！", "确定");
            // }
        }


        public void OpenJsonConfigPath()
        {
            string folderPath = "Assets/Config/Processs/";
            EditorUtility.RevealInFinder(folderPath);
        }
        
        private void OnDelClick()
        {
            if (EditorUtility.DisplayDialog("警告", $"确定删除{this.m_Graph.name}?", "确定", "取消"))
            {
                bool isSame = this.m_Graph == this.m_Window.Graph;
                var path = AssetDatabase.GetAssetPath(this.m_Graph);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh();

                m_Window.InitializeGraph(isSame ? ProcessUtils.GetAllProcess()[0] : this.m_Window.Graph);
                m_Window.GraphView.FileView.Repaint();
            }
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var rect = GetPosition();
            Cookie.SetPublic(WIDTH, rect.width);
            Cookie.SetPublic(HEIGHT, rect.height);
            Cookie.SetPublic(POSX, rect.position.x);
            Cookie.SetPublic(POSY, rect.position.y);
        }
        
    }
}