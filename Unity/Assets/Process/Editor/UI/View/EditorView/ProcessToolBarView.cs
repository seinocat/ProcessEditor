using System.IO;
using GraphProcessor;
using Process.Runtime;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Process.Editor
{
    public class ProcessToolBarView : ToolbarView
    {
        private ProcessGraphView m_GraphView;
        private ToolbarButtonData m_FormatButton;
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
            AddToggle(new GUIContent("小地图"), Cookie.GetPublic(SHOW_MINIMAP, true), ShowMiniMap);
            AddButton(new GUIContent("定位"), LocateFile);
            AddToggle(new GUIContent("Runtime Debug"), false, RuntimeDebug);
            AddButton(new GUIContent("计算Order"), () => { m_GraphView.Window.Graph.ComputeGraphOrder(); });
            m_FormatButton = AddButton(new GUIContent($"运行时格式:{ProcessRuntimeFormatSettings.GetFormat()}"), ToggleExportFormat);
            AddButton(new GUIContent("打开导出目录"), OpenExportFolder);
            AddButton(new GUIContent("导出当前"), Export, false);
            AddButton(new GUIContent("代码生成"), OneKeyGenerate, false);
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

        public void RuntimeDebug(bool open)
        {
            m_GraphView.graph.RuntimeDebug = open;
            m_GraphView.nodeViews.ForEach(x => x.UpdateRuntimeIconView());
        }

        public void Refresh()
        {
            m_GraphView.FileView.Repaint();
            CompilationPipeline.RequestScriptCompilation();
        }

        private void ToggleExportFormat()
        {
            var current = ProcessRuntimeFormatSettings.GetFormat();
            var next = current == ProcessConfigFormat.Binary ? ProcessConfigFormat.Json : ProcessConfigFormat.Binary;
            ProcessRuntimeFormatSettings.SetFormat(next);
            if (m_FormatButton?.content != null)
                m_FormatButton.content.text = $"运行时格式:{next}";
            m_GraphView?.Window?.Repaint();
            EditorUtility.DisplayDialog("提示", $"当前运行时格式切换为: {next}", "确定");
        }

        public void Export()
        {
            EditorUtility.DisplayDialog("提示",
                ProcessExportUtils.ExportAllProcess() ? "导出流程配置成功" : "导出失败，请根据报错日志修改！", "确定");
        }

        private void OpenExportFolder()
        {
            var exportPath = Application.streamingAssetsPath;
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
                AssetDatabase.Refresh();
            }
            
            var format = ProcessRuntimeFormatSettings.GetFormat();
            var fileName = format == ProcessConfigFormat.Json ? "Events.json" : "Events.bytes";
            var targetFile = Path.Combine(exportPath, fileName);
            if (File.Exists(targetFile))
            {
                EditorUtility.RevealInFinder(targetFile);
                return;
            }

            var manifestPath = Path.Combine(exportPath, "Events.manifest.json");
            if (File.Exists(manifestPath))
            {
                EditorUtility.RevealInFinder(manifestPath);
                return;
            }

            EditorUtility.RevealInFinder(exportPath);
        }

        public void OneKeyGenerate()
        {
            AutoGenerator.OneKeyGenerate();
        }
    }
}
