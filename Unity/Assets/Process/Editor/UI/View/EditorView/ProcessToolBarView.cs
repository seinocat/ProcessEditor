using GraphProcessor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Process.Editor
{
    public class ProcessToolBarView : ToolbarView
    {
        private ProcessGraphView m_GraphView;
        private const string SHOW_OPERATION = "ProcessEditor_ShowOperation";
        private const string SHOW_FILELIST = "ProcessEditor_ShowFileList";
        private const string SHOW_MINIMAP = "ProcessEditor_MiniMap";
        
        public ProcessToolBarView(BaseGraphView graphView) : base(graphView)
        {
            m_GraphView = graphView as ProcessGraphView;
            ShowFileList(Cookie.GetPublic(SHOW_FILELIST, true));
            ShowMiniMap(Cookie.GetPublic(SHOW_MINIMAP, false));
        }

        protected override void AddButtons()
        {
            AddToggle(new GUIContent("配置列表"), Cookie.GetPublic(SHOW_FILELIST, true), ShowFileList);
            // AddToggle(new GUIContent("导出面板"), Cookie.GetPublic(SHOW_OPERATION, true), ShowOperation);
            AddToggle(new GUIContent("小地图"), Cookie.GetPublic(SHOW_MINIMAP, true), ShowMiniMap);
            AddButton(new GUIContent("定位"), LocateFile);
            AddButton(new GUIContent("计算Order"), ()=> {m_GraphView.Window.Graph.ComputeGraphOrder();});
            AddButton(new GUIContent("刷新"), Refresh);
            AddButton(new GUIContent("导出当前"), Export, false);
            AddButton(new GUIContent("一键导出"), OneKeyExport, false);
        }
        
        public void ShowFileList(bool show)
        {
            if (show)
            {
                Cookie.SetPublic(SHOW_FILELIST, true);
                m_GraphView.DrawFileListView();
            }
            else
            {
                Cookie.SetPublic(SHOW_FILELIST, false);
                m_GraphView.DelFileListView();
            }
        }

        public void ShowMiniMap(bool show)
        {
            if (show)
            {
                Cookie.SetPublic(SHOW_MINIMAP, true);
                m_GraphView.DrawMiniMapView();
            }
            else
            {
                Cookie.SetPublic(SHOW_MINIMAP, false);
                m_GraphView.DelMiniMapView();
            }
        }
        
        public void LocateFile()
        {
            m_GraphView.FileView.LocateSelect();
        }

        public void Refresh()
        {
            m_GraphView.FileView.Repaint();
            CompilationPipeline.RequestScriptCompilation();
        }

        public void Export()
        {
            EditorUtility.DisplayDialog("提示",
                ProcessExportUtils.ExportAllProcess() ? "导出流程配置成功" : "导出失败，请根据报错日志修改！", "确定");
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
        
    }
}