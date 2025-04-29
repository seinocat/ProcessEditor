using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Process.Editor
{
    public class GraphItemView : VisualElement
    {
        public ProcessGraphBase     GraphBase;
        public GraphItemData        m_Data;
        private ProcessGraphWindow  m_Window;
        private FileListView        m_FileView;

        private Label m_FolderCollapse;
        private Image m_FolderIcon;
        private List<GraphItemView> m_SubViews;
        private int m_Indent;
        private string CollapseKey => $"ProcessEvent_Collapse_{m_Data.ProcessPath}";
        
        private DateTime lastClickTime;
        private float doubleClickThreshold = 0.4f;
        private Action<GraphItemView, ContextualMenuPopulateEvent> m_MenuPopulateAction;

        public bool Collapse { get; private set; }
        public bool IsSelected { get; private set; }

        public static GraphItemView CreateFolderView(FileListView fileView, ProcessGraphWindow window, int indent, string path)
        {
            GraphItemData data = GraphItemData.CreateDictory(path);
            GraphItemView view = new GraphItemView(fileView, window, indent, data);
            
            return view;
        }
        
        public static GraphItemView CreateGraphView(FileListView fileView, ProcessGraphWindow window, int indent, ProcessGraphBase graph)
        {
            GraphItemData data = GraphItemData.CreateGraph(graph);
            GraphItemView view = new GraphItemView(fileView, window, indent, data, graph);
            return view;
        }
        
        public GraphItemView(FileListView fileView, ProcessGraphWindow window, int indent, GraphItemData data, ProcessGraphBase graph = null)
        {
            m_SubViews  = new List<GraphItemView>();
            GraphBase   = graph;
            m_Window    = window;
            m_FileView  = fileView;
            m_Data      = data;
            m_Indent    = indent;
            Collapse    = Cookie.GetPublic(CollapseKey, 0) == 0;
            m_MenuPopulateAction    = fileView.MenuPopulate;
            style.flexDirection     = FlexDirection.Row;
            style.justifyContent    = Justify.FlexStart;

            RegisterCallback<PointerDownEvent>((pointDown) =>
            {
                if (pointDown.button == 0)
                {
                    m_FileView.SetSelected(this);
                
                    // 获取当前点击的时间戳
                    DateTime currentClickTime = DateTime.Now;
                    TimeSpan timeSinceLastClick = currentClickTime - lastClickTime;

                    if (timeSinceLastClick.TotalSeconds < doubleClickThreshold)
                    {
                        if (m_Data.IsFolder)  DoCollapse(!Collapse);
                        else                  OpenBtnClick();
                        
                        // 重置上一次点击的时间戳
                        lastClickTime = DateTime.MinValue;
                    }
                    else
                    {
                        // 更新上一次点击的时间戳
                        lastClickTime = currentClickTime;
                    }
                }

                if (pointDown.button == 1)
                {
                    if (!IsSelected)
                    {
                        m_FileView.SetSelected(this);
                    }
                }
            });
            
            this.AddManipulator(new ContextualMenuManipulator(BuildContextMenu));
        }

        public void SetSelect(bool value)
        {
            IsSelected = value;
            style.backgroundColor = value ? new Color(0.22f, 0.4f, 0.67f) : new Color(0f,0f,0f,0f);
        }
        
        private void BuildContextMenu(ContextualMenuPopulateEvent evt)
        {
            m_MenuPopulateAction?.Invoke(this, evt);
        }

        public void DrawView()
        {
            if (m_Data.IsFolder)
            {
                DrawFolder();
                if (!Collapse)
                {
                    DrawSubFolder();
                    DrawSubGraph();
                }
            }
            else
            {
                DrawGraph();
            }
        }

        public void DoCollapse(bool value)
        {
            if (Collapse == value) return;
            
            Collapse = value;
            Cookie.SetPublic(CollapseKey, Collapse ? 0 : 1);
            if (ExistSubFolderOrFile())
                m_FolderCollapse.text = Collapse ? "▶" : "▼";
            else
                m_FolderCollapse.text = "    ";
            m_FolderIcon.sprite = Collapse ? m_Window.GraphView.FolderClose : m_Window.GraphView.FolderOpen;
            if (!Collapse)
            {
                DrawSubFolder();
                DrawSubGraph();
            }
            else
            {
                ClearSubView();
            }
        }

        private void DrawFolder()
        {
            contentContainer.style.alignItems = Align.Center;
            contentContainer.style.justifyContent = Justify.Center;
            
            m_FolderCollapse = new Label();
            m_FolderCollapse.style.fontSize = 8;
            if (ExistSubFolderOrFile())
                m_FolderCollapse.text = Collapse ? "▶" : "▼";
            else
                m_FolderCollapse.text = "    ";
            m_FolderCollapse.style.marginLeft = 20 * m_Indent;
            Add(m_FolderCollapse);
            
            m_FolderCollapse.RegisterCallback<PointerDownEvent>((pointDown) =>
            {
                if (m_Data.IsFolder)
                {
                    DoCollapse(!Collapse);
                }
            });
            
            m_FolderIcon = new Image();
            m_FolderIcon.sprite = Collapse ? m_Window.GraphView.FolderClose : m_Window.GraphView.FolderOpen;
            m_FolderIcon.style.width = 15;
            m_FolderIcon.style.height = 15;
            Add(m_FolderIcon);

            Label graphName = new Label();
            graphName.text = m_Data.Name;
            graphName.style.fontSize = 12;
            graphName.style.flexGrow = 1;
            Add(graphName);
        }
        
        private bool ExistSubFolderOrFile()
        {
            var folders = ProcessUtils.GetSubFolders(m_Data.Path);
            var graphs = ProcessUtils.LoadAllAssets<ProcessGraphBase>(m_Data.Path, SearchOption.TopDirectoryOnly);
            return folders.Count > 0 || graphs.Count > 0;
        }

        
        private void DrawGraph()
        {
            contentContainer.style.alignItems = Align.Center;
            contentContainer.style.justifyContent = Justify.Center;
            
            Image graphIcon = new Image();
            graphIcon.sprite = m_Window.GraphView.GraphIcon;
            graphIcon.style.marginLeft = 20 * m_Indent;
            graphIcon.style.width = 15;
            graphIcon.style.height = 15;
            Add(graphIcon);
            
            Label graphName = new Label();
            graphName.text = GraphBase.name;
            graphName.style.flexGrow = 1;
            
            if (m_Window.Graph == GraphBase)
            {
                graphName.style.color = Color.cyan;
            }
            Add(graphName);
        }
        
        private void DrawSubFolder()
        {
            var curIndex = m_FileView.contentContainer.IndexOf(this);
            var folders = ProcessUtils.GetSubFolders(m_Data.Path);
            curIndex++;
            for (int i = 0; i < folders.Count; i++)
            {
                var view = CreateFolderView(m_FileView, m_Window, m_Indent + 1, folders[i]);
                m_SubViews.Add(view);
                m_FileView.Insert(curIndex, view);
                view.DrawView();
                if (view.m_SubViews.Count > 0)
                {
                    curIndex += view.GetSubViewCount() + 1;
                }
                else
                {
                    curIndex++;
                }
                
            }
        }

        private void DrawSubGraph()
        {
            var curIndex = m_FileView.IndexOf(this) + m_SubViews.Count + 1;
            var graphs = ProcessUtils.LoadAllAssets<ProcessGraphBase>(m_Data.Path, SearchOption.TopDirectoryOnly);
            graphs.Sort((x, y) => string.Compare(x.name, y.name, StringComparison.Ordinal));
            
            for (int i = 0; i < graphs.Count; i++)
            {
                var view = CreateGraphView(m_FileView, m_Window, m_Indent + 1, graphs[i]);
                m_SubViews.Add(view);
                m_FileView.Insert(curIndex, view);
                view.DrawView();
                if (view.m_SubViews.Count > 0)
                {
                    curIndex += view.GetSubViewCount() + 1;
                }
                else
                {
                    curIndex++;
                }
            }
        }

        private void ClearSubView()
        {
            for (int i = 0; i < m_SubViews.Count; i++)
            {
                m_SubViews[i].ClearSubView();
                m_FileView.Remove(m_SubViews[i]);
            }
            m_SubViews.Clear();
        }
        
        public int GetSubViewCount()
        {
            return m_SubViews.Count + m_SubViews.Sum(view => view.GetSubViewCount());
        }

        private void OpenBtnClick()
        {
            if (m_Window.Graph != GraphBase)
            {
                m_FileView.SetScrollOffset();
                Cookie.SetPublic(ProcessGraphWindow.OPEN_GRAPH, GraphBase.name);
                m_Window.InitializeGraph(GraphBase);
            }
        }


    }
}