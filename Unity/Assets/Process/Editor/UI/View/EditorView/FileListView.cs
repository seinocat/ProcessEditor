using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Seino.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace Process.Editor
{
    public sealed class FileListView : Blackboard
    {
        #region 字段
        
        public static Vector2 m_Offset;
        public static List<ProcessConfigEditorNode> ExportData = new();
        
        private ProcessGraphWindow m_Window;
        private SearchView m_SearchView;
        private ScrollView m_Scroll;
        private GraphItemView m_CurSelected;
        private List<GraphItemView> m_SelectedList = new();
        private List<ProcessGraphBase> m_GraphsList = new();
        private bool m_Shift;
        private bool m_Ctrl;
        private const string WIDTH = "ProcessEditor_FileList_Width";
        private const string HEIGHT = "ProcessEditor_FileList_Heigh";
        
        #endregion
        
        #region 构造

        public FileListView(ProcessGraphView graphView)
        {
            m_Window = graphView.Window;
            m_SearchView = new SearchView(graphView);
            scrollable = true;
            addItemRequested = OnAddBtnClick;
            FieldInfo fieldInfo = typeof(Blackboard).GetField("m_ScrollView", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null) m_Scroll = fieldInfo.GetValue(this) as ScrollView;
            var width = Cookie.GetPublic(WIDTH, 425f);
            var height = Cookie.GetPublic(HEIGHT, 1000f);
            SetPosition(new Rect(0, 23, width, height));
            Register();
            DrawView();
        }

        private void Register()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<KeyUpEvent>((evt) =>
            {
                if (!evt.shiftKey) m_Shift = false;
                if (!evt.ctrlKey) m_Ctrl = false;
                
                if (evt.keyCode == KeyCode.Return)
                {
                    if (m_CurSelected != null && !m_CurSelected.m_Data.IsFolder && m_Window.Graph != m_CurSelected.GraphBase)
                    {
                        SetScrollOffset();
                        m_Window.InitializeGraph(m_CurSelected.GraphBase);
                    }
                }
                if (evt.keyCode == KeyCode.Delete)
                {
                    BatchDelete();
                }
            });
            
            RegisterCallback<KeyDownEvent>((evt) =>
            {
                if (evt.keyCode == KeyCode.UpArrow)
                {
                    if (m_CurSelected != null)
                    {
                        int index = contentContainer.IndexOf(m_CurSelected);
                        index = Mathf.Max(0, --index);
                        SetSelected(contentContainer[index] as GraphItemView);
                    }
                }
                if (evt.keyCode == KeyCode.DownArrow)
                {
                    if (m_CurSelected != null)
                    {
                        int index = contentContainer.IndexOf(m_CurSelected);
                        index = Mathf.Min(contentContainer.childCount - 1, ++index);
                        SetSelected(contentContainer[index] as GraphItemView);
                    }
                }
                if (evt.keyCode == KeyCode.LeftArrow)
                {
                    if (m_CurSelected != null)
                    {
                        if (m_CurSelected.m_Data.IsFolder)
                        {
                            if (m_CurSelected.Collapse)
                            {
                                var index = IndexOf(m_CurSelected);
                                var start = Mathf.Max(0, index - 1);
                                for (int i = start; i >= 0; i--)
                                {
                                    if (contentContainer[i] is GraphItemView itemView)
                                    {
                                        if (itemView.m_Data.IsFolder && !itemView.Collapse)
                                        {
                                            SetSelected(itemView);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                m_CurSelected.DoCollapse(true);
                            }
                        }
                        else
                        {
                            var index = IndexOf(m_CurSelected);
                            var start = Mathf.Max(0, index - 1);
                            for (int i = start; i >= 0; i--)
                            {
                                if (contentContainer[i] is GraphItemView itemView)
                                {
                                    if (itemView.m_Data.IsFolder)
                                    {
                                        SetSelected(itemView);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (evt.keyCode == KeyCode.RightArrow)
                {
                    if (m_CurSelected != null)
                    {
                        if (m_CurSelected.m_Data.IsFolder)
                        {
                            if (!m_CurSelected.Collapse)
                            {
                                var index = IndexOf(m_CurSelected);
                                var start = Mathf.Min(childCount - 1, index + 1);
                                for (int i = start; i < childCount; i++)
                                {
                                    if (contentContainer[i] is GraphItemView itemView)
                                    {
                                        if (itemView.m_Data.IsFolder && itemView.Collapse)
                                        {
                                            SetSelected(itemView);
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                m_CurSelected.DoCollapse(false);
                            } 
                        }
                        else
                        {
                            var index = IndexOf(m_CurSelected);
                            var start = Mathf.Min(childCount - 1, index + 1);
                            for (int i = start; i < childCount; i++)
                            {
                                if (contentContainer[i] is GraphItemView itemView)
                                {
                                    if (itemView.m_Data.IsFolder)
                                    {
                                        SetSelected(itemView);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (evt.shiftKey) m_Shift = true;
                if (evt.ctrlKey) m_Ctrl = true;
            });
        }
        
        private void DrawView()
        {
            m_GraphsList = ProcessUtils.LoadAllAssets<ProcessGraphBase>(GlobalPathConfig.GraphsAssetsPath);
            contentContainer.Clear();
            ExportData.Clear();
            ClearSelected();
            var searchkey = m_SearchView.m_InputText;
            Add(m_SearchView);

            foreach (var graphBase in m_GraphsList)
            {
                foreach (var node in graphBase.nodes)
                {
                    if (node is ProcessConfigEditorNode eventNode)
                    {
                        ExportData.Add(eventNode);
                    }
                }
                
            }

            if (string.IsNullOrEmpty(searchkey))
            {
                DrawHierarchy();
            }
            else
            {
                DrawSearch(searchkey);
            }
            
            if (m_Window.Graph != null)
            {
                if (m_CurSelected == null)
                {
                    foreach (var view in contentContainer.Children())
                    {
                        if (view is GraphItemView item)
                        {
                            if (item.GraphBase == m_Window.Graph)
                            {
                                SetSelected(item);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var view in contentContainer.Children())
                    {
                        if (view is GraphItemView item)
                        {
                            if (item.GraphBase == m_CurSelected.GraphBase)
                            {
                                SetSelected(item);
                                break;
                            }
                        }
                    }
                    
                }
            }
            //Scroll定位
            m_Scroll.scrollOffset = m_Offset;
        }
        
        private void DrawHierarchy()
        {
            var folders = ProcessUtils.GetSubFolders(GlobalPathConfig.GraphsAssetsPath);
            for (int i = 0; i < folders.Count; i++)
            {
                var view = GraphItemView.CreateFolderView(this, m_Window, 0, folders[i]);
                contentContainer.Add(view);
                view.DrawView();
            }
            
            var graphBases = ProcessUtils.LoadAllAssets<ProcessGraphBase>(GlobalPathConfig.GraphsAssetsPath, SearchOption.TopDirectoryOnly);
            for (int i = 0; i < graphBases.Count; i++)
            {
                var view = GraphItemView.CreateGraphView(this, m_Window, 0, graphBases[i]);
                contentContainer.Add(view);
                view.DrawView();
            }
        }

        private void DrawSearch(string searchkey)
        {
            List<GraphItemView> itemList = new List<GraphItemView>();
            for (int i = 0; i < m_GraphsList.Count; i++)
            {
                var graphItem = m_GraphsList[i];
                bool isTarget = Regex.IsMatch(graphItem.name, searchkey, RegexOptions.IgnoreCase);

                if (!isTarget)
                {
                    var eventNode = graphItem.GetNode<ProcessConfigEditorNode>();
                    foreach (var node in eventNode)
                    {
                        if (Regex.IsMatch(node.ProcessId.ToString(), searchkey, RegexOptions.IgnoreCase))
                            isTarget = true;
                        if (Regex.IsMatch(node.ProcessDesc, searchkey, RegexOptions.IgnoreCase))
                            isTarget = true;
                    }

                    bool nodetype = searchkey.StartsWith("n:");
                    if (nodetype)
                    {
                        string nodename = searchkey.Split("n:")[1];
                        if (nodename.IsNullOrEmpty()) return;
                        var nodes = graphItem.GetNode<ProcessEditorNodeBase>();
                        foreach (var node in nodes)
                        {
                            if (Regex.IsMatch(node.name, nodename, RegexOptions.IgnoreCase))
                            {
                                isTarget = true;
                                break;
                            }
                            if (Regex.IsMatch(node.GetType().Name, nodename, RegexOptions.IgnoreCase))
                            {
                                isTarget = true;
                                break;
                            }
                        }
                    }
                }
                
                if (!isTarget) continue;

                var graphItemView = GraphItemView.CreateGraphView(this, m_Window, 0, graphItem);
                itemList.Add(graphItemView);
            }
            
            itemList.Sort((x,y)=>string.Compare(x.name, y.name, StringComparison.Ordinal));
            itemList.ForEach(x =>
            {
                Add(x);
                x.DrawView();
            });
        }

        
        
        #endregion
        
        #region 公开

        public bool IsGraphExist(ProcessGraphBase graph)
        {
            if (m_GraphsList.Contains(graph) || m_GraphsList.Exists(x=>x.name == graph.name))
            {
                return true;
            }

            return false;
        }
        
        public bool IsGraphExist(string graphName)
        {
            if (m_GraphsList.Exists(x=>x.name == graphName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 重绘面板
        /// </summary>
        public void Repaint()
        {
            SetScrollOffset();
            DrawView();
        }
        
        /// <summary>
        /// 滑动条定位
        /// </summary>
        public void SetScrollOffset()
        {
            m_Offset = m_Scroll.scrollOffset;
        }

        public void SetSelected(GraphItemView view)
        {
            if (m_CurSelected == view && m_SelectedList.Count == 1) return;
            
            if (m_CurSelected != null  && !m_Ctrl && !m_Shift)
            {
                ClearSelected();
            }

            if (view == null) return;
            
            if (m_CurSelected != null && m_Ctrl && !m_Shift)
            {
                AddSelected(view);
            }
            else if (m_CurSelected != null && !m_Ctrl && m_Shift)
            {
                var curIndex = IndexOf(m_CurSelected);
                var newIndex = IndexOf(view);
                if (newIndex > curIndex)
                {
                    for (int i = curIndex + 1; i <= newIndex; i++)
                    {
                        if (contentContainer[i] is GraphItemView itemView)
                        {
                            AddSelected(itemView);
                        }
                    }
                }
                else
                {
                    for (int i = newIndex; i <= curIndex; i++)
                    {
                        if (contentContainer[i] is GraphItemView itemView)
                        {
                            AddSelected(itemView);
                        }
                    }
                }
                
            }
            else
            {
                AddSelected(view);
            }
            
            m_CurSelected = view;
        }

        public void AddSelected(GraphItemView view)
        {
            if (!m_SelectedList.Contains(view))
            {
                view.SetSelect(true);
                m_SelectedList.Add(view);
            }
        }

        public void RemoveSelected(GraphItemView view)
        {
            if (m_SelectedList.Contains(view))
            {
                view.SetSelect(false);
                m_SelectedList.Remove(view);
            }
        }

        public void ClearSelected()
        {
            for (int i = m_SelectedList.Count - 1; i >= 0; i--)
            {
                var view = m_SelectedList[i];
                view.SetSelect(false);
                m_SelectedList.Remove(view);
            }
        }
        
        public void LocateSelect()
        {
            if (m_CurSelected == null) return;

            var folders = ProcessUtils.GetParentFolders(m_CurSelected.m_Data.ProcessPath);
            foreach (var folder in folders)
            {
                Cookie.SetPublic($"ProcessEvent_Collapse_{folder}", 1);
            }

            m_SearchView.ClearText();
        }
        
        #endregion
        
        #region 菜单方法
        
        public void MenuPopulate(GraphItemView view, ContextualMenuPopulateEvent evt)
        {
            // 清空默认的右键菜单
            evt.menu.MenuItems().Clear();
            if (m_SelectedList.Count <= 1)
            {
                if (view.m_Data.IsFolder)
                    DrawFolderMenu(view, evt);
                else
                    DrawGraphMenu(view, evt);
            }
            else
            {
                DrawBatchMenu(view, evt);
            }
            
            // 阻止默认的右键菜单显示
            evt.StopPropagation();
        }

        public void DrawFolderMenu(GraphItemView view, ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("导出当前文件夹", action=>
            {
                FolderExport(view);
            });
                    
            evt.menu.AppendAction("导出当前文件夹(包含子文件夹)", action=>
            {
                FolderExport(view, false);
            });
                
            evt.menu.AppendAction("新建流程", action=>
            {
                AddEvent(view);
            });
                
            evt.menu.AppendAction("新建文件夹", action=>
            {
                AddFolder(view);
            });
                
            evt.menu.AppendAction("新建文件夹(根目录)", action=>
            {
                AddFolder(view, true);
            });
                    
            evt.menu.AppendAction("复制名称", action =>
            {
                CopyTextToClipboard(view);
            });

            evt.menu.AppendAction("重命名", action=>
            {
                RenameFolder(view);
            });
                
            evt.menu.AppendAction("文件资源管理器中显示", action=>
            {
                ShowInExplorer(view);
            });
                
            evt.menu.AppendAction("删除文件夹", action=>
            {
                DeleteFolder(view);
            });
        }

        public void DrawGraphMenu(GraphItemView view, ContextualMenuPopulateEvent evt)
        {
            // 添加自定义的右键菜单项
            evt.menu.AppendAction("导出", action =>
            {
                Export(view);
            });
                    
            evt.menu.AppendAction("复制名称", action =>
            {
                CopyTextToClipboard(view);
            });
                
            evt.menu.AppendAction("重命名", action =>
            {
                RenameGraph(view);
            });
                
            evt.menu.AppendAction("文件资源管理器中显示", action=>
            {
                ShowInExplorer(view);
            });
                
            evt.menu.AppendAction("删除", action =>
            {
                DeleteGraph(view);
            });
        }

        public void DrawBatchMenu(GraphItemView view, ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("批量导出", action =>
            {
                BatchExport();
            });
                
            evt.menu.AppendAction("批量导出(包含子文件夹)", action =>
            {
                BatchExport(false);
            });
                
            evt.menu.AppendAction("批量删除", action =>
            {
                BatchDelete();
            });
        }
        
        private void OnAddBtnClick(Blackboard blackboard)
        {
            CustomGameEventWindow.OpenWindow("创建新流程", "New Process File", (CreateName) =>
            {
                var graph = ScriptableObject.CreateInstance<ProcessGraphBase>();
                var path = $"{GlobalPathConfig.GraphsAssetsPath}/{CreateName}.asset";
                if (!IsGraphExist(CreateName))
                {
                    AssetDatabase.CreateAsset(graph, path);
                    AssetDatabase.Refresh();
                    Repaint();
                    CustomGameEventWindow.CloseWindow();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该配置已存在!", "确定");
                }
            });
        }

        private void ShowInExplorer(GraphItemView view)
        {
            var path =  view.m_Data.DiskPath;
            path = path.Replace("/", "\\");
            if (view.m_Data.IsFolder)
            {
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", "/select," + path);
            }
        }

        private void CopyTextToClipboard(GraphItemView view)
        {
            GUIUtility.systemCopyBuffer = view.m_Data.Name;
        }
        
        private void AddEvent(GraphItemView view)
        {
            CustomGameEventWindow.OpenWindow("创建新流程", "New Process File", (CreateName) =>
            {
                var graph = ScriptableObject.CreateInstance<ProcessGraphBase>();
                var path = Path.Combine(view.m_Data.Path, $"{CreateName}.asset");
                if (!IsGraphExist(CreateName))
                {
                    AssetDatabase.CreateAsset(graph, path);
                    AssetDatabase.Refresh();
                    CustomGameEventWindow.CloseWindow();
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该配置已存在!", "确定");
                }
            });
        }

        private void AddFolder(GraphItemView view, bool root = false)
        {
            CustomGameEventWindow.OpenWindow("创建文件夹", "New Folder", (CreateName) =>
            {
                var path = root ? Path.Combine(Application.dataPath, GlobalPathConfig.GraphsRelativePath) : view.m_Data.DiskPath;
                path = Path.Combine(path, $"{CreateName}");
                Debug.LogError(path);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    CustomGameEventWindow.CloseWindow();
                    AssetDatabase.Refresh();
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该文件夹已存在!", "确定");
                }
            });
        }

        private void RenameFolder(GraphItemView view)
        {
            CustomGameEventWindow.OpenWindow("重命名文件夹", view.m_Data.Name, (NewName) =>
            {
                var path = view.m_Data.DiskPath;
                var newPath = path.Replace(view.m_Data.Name, NewName);
                if (!Directory.Exists(newPath))
                {
                    //移动新文件夹下
                    Directory.Move(path, newPath);
                    AssetDatabase.Refresh();
                    //删除原来的文件夹、
                    if (Directory.Exists(path))
                    {
                        //删除文件夹的meta文件
                        if (File.Exists($"{path}.meta"))
                        {
                            File.Delete($"{path}.meta");
                        }

                        Directory.Delete(path, true);
                    }
                    AssetDatabase.Refresh();
                    CustomGameEventWindow.CloseWindow();
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该文件夹已存在!", "确定");
                }
            });
        }

        private void DeleteFolder(GraphItemView view, bool tip = true, bool refresh = true)
        {
            var path = view.m_Data.DiskPath;
            void execute()
            {
                if (Directory.Exists(path))
                {
                    //先删除文件夹的meta文件
                    if (File.Exists($"{path}.meta"))
                    {
                        File.Delete($"{path}.meta");
                    }
                    Directory.Delete(path, true);
                    CustomGameEventWindow.CloseWindow();
                    if (refresh) AssetDatabase.Refresh();
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "文件夹不存在!", "确定");
                }
            }
            
            if (tip)
            {
                if (EditorUtility.DisplayDialog("警告", $"确定删除{ view.m_Data.Name}?", "确定", "取消"))
                {
                    execute();
                }
            }
            else
            {
                execute();
            }
        }
        
        private void Export(GraphItemView view)
        {
            // if (ProcessExportUtils.ExportGraph(view.m_Graph))
            // {
            //     EditorUtility.DisplayDialog("提示", $"导出{view.m_Graph.name}成功", "确定");
            // }
            // else
            // {
            //     EditorUtility.DisplayDialog("提示", $"导出{view.m_Graph.name}失败，请根据报错日志修改！", "确定");
            // }
        }

        private void FolderExport(GraphItemView view, bool top = true)
        {
            // var graphBases =ProcessEditorUtils.LoadAllAssets<ProcessGraphBase>(view.m_Data.DiskPath, top ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
            // bool success = true;
            // for (int i = 0; i < graphBases.Count; i++)
            // {
            //     var graph = graphBases[i];
            //     EditorUtility.DisplayProgressBar("批量导出",
            //         $"正在导出{graph.name}({i + 1}/{graphBases.Count})",
            //         (i + 1) / (float)graphBases.Count);
            //     if (!GameEventExportUtils.ExportGraphNonRefresh(graph))
            //     {
            //         success = false;
            //         EditorUtility.DisplayDialog("提示", $"导出{graph.name}失败，请根据报错日志修改！", "确定");
            //         break;
            //     }
            // }
            //
            // EditorUtility.ClearProgressBar();
            // AssetDatabase.Refresh();
            // if (success)
            // {
            //     EditorUtility.DisplayDialog("提示", $"批量导出成功, 数量: {graphBases.Count}", "确定");
            // }
        }
        
        private void RenameGraph(GraphItemView view)
        {
            CustomGameEventWindow.OpenWindow("重命名", view.GraphBase.name, (NewName) =>
            {
                var oldePath = view.m_Data.Path;
                if (IsGraphExist(NewName))
                {
                    EditorUtility.DisplayDialog("提示", "该配置已存在!", "确定");
                }
                else
                {
                    AssetDatabase.RenameAsset(oldePath, NewName);
                    AssetDatabase.Refresh();
                    Repaint();
                    CustomGameEventWindow.CloseWindow();
                }
            });
        }
        
        private void DeleteGraph(GraphItemView view, bool tip = true, bool refresh = true)
        {
            void execute()
            {
                bool isSame = view.GraphBase == m_Window.Graph;
                var path = AssetDatabase.GetAssetPath(view.GraphBase);
                AssetDatabase.DeleteAsset(path);

                if (refresh) AssetDatabase.Refresh();
                m_Window.InitializeGraph(isSame ? ProcessUtils.GetAllProcess()[0] : m_Window.Graph);
                Repaint();
            }
            
            if (tip)
            {
                if (EditorUtility.DisplayDialog("警告", $"确定删除{view.GraphBase.name}?", "确定", "取消"))
                {
                    execute();
                }
            }
            else
            {
                execute();
            }
        }
        
        private void BatchExport(bool top = true)
        {
            
        }

        private void BatchDelete()
        {
            var deleteList = m_SelectedList.FindAll(x => !x.m_Data.IsFolder);
            if (EditorUtility.DisplayDialog("警告", $"确定删除所选文件?数量:{deleteList.Count}", "确定", "取消"))
            {
                for (int i = deleteList.Count - 1; i >= 0 ; i--)
                {
                    var view = deleteList[i];
                    RemoveSelected(view);
                    if (view.m_Data.IsFolder)
                        DeleteFolder(view);
                    else
                        DeleteGraph(view);
                }
                AssetDatabase.Refresh();
                Repaint();
            }
        }

        #endregion
        
        #region Editor

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var rect = GetPosition();
            Cookie.SetPublic(WIDTH, rect.width);
            Cookie.SetPublic(HEIGHT, rect.height);
            SetPosition(new Rect(0, 23, rect.width, rect.height));
        }

        #endregion
        
    }
}