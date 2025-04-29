using System;
using System.Collections.Generic;
using System.Reflection;
using GraphProcessor;
using ProcessEditor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Process.Editor
{
    public class ProcessGraphView : BaseGraphView
    {
        private class GraphGridView : GridBackground { }

        public ProcessGraphWindow Window;
        public FileListView FileView;
        public OperationView OperationView;
        public ProcessMiniMapView MiniMapView;
        
        public Sprite FolderOpen;
        public Sprite FolderClose;
        public Sprite GraphIcon;

        public ProcessGraphView(ProcessGraphWindow window) : base(window)
        {
            Window = window;
            FolderOpen = AssetDatabase.LoadAssetAtPath<Sprite>($"{GlobalPathConfig.ResIconPath}/folder1.png");
            FolderClose = AssetDatabase.LoadAssetAtPath<Sprite>($"{GlobalPathConfig.ResIconPath}/folder0.png");
            GraphIcon = AssetDatabase.LoadAssetAtPath<Sprite>($"{GlobalPathConfig.ResIconPath}/graph.png");
            
            Insert(0, new GraphGridView());
        }

        public void DrawOperationView()
        {
            OperationView = new OperationView(this);
            Add(OperationView);
        }

        public void DelOperationView()
        {
            if (this.OperationView != null)
            {
                Remove(this.OperationView);
            }
        }

        public void DrawFileListView()
        {
            this.FileView = new FileListView(this);
            Add(this.FileView);
        }
        
        public void DelFileListView()
        {
            if (this.FileView != null)
            {
                Remove(this.FileView);
            }
        }

        public void DrawMiniMapView()
        {
            this.MiniMapView = new ProcessMiniMapView();
            Add(this.MiniMapView);
        }
        
        public void DelMiniMapView()
        {
            if (this.MiniMapView != null)
            {
                Remove(this.MiniMapView);
            }
        }
        
        public override IEnumerable<(string path, Type type)> FilterCreateNodeMenuEntries()
        {
            foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries(graph))
            {
                var eventNodeAttr = nodeMenuItem.type.GetCustomAttribute<ProcessNodeAttribute>();
                var discardAttr = nodeMenuItem.type.GetCustomAttribute<DiscardNodeAttribute>();

                if (eventNodeAttr != null && discardAttr == null)
                {
                    yield return nodeMenuItem;
                }
            }
        }
    }
}