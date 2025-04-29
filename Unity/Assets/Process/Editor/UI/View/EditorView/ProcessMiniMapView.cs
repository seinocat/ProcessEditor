using Process.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Process.Editor
{
    public sealed class ProcessMiniMapView : MiniMap
    {
        private const string POSX = "ProcessEditor_MiniMap_PosX";
        private const string POSY = "ProcessEditor_MiniMap_PosY";

        public ProcessMiniMapView()
        {
            var posx = Cookie.GetPublic(POSX, 0f);
            var posy = Cookie.GetPublic(POSY, 200f);
            SetPosition(new Rect(posx, posy, 300, 200));
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            var rect = GetPosition();
            Cookie.SetPublic(POSX, rect.position.x);
            Cookie.SetPublic(POSY, rect.position.y);
        }
    }
}