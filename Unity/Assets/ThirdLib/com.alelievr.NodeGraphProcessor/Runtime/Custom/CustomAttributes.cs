using System;

namespace GraphProcessor
{
    public partial class NodeMenuItemAttribute
    {
        public NodeMenuItemAttribute(int id)
        {
            menuTitle = NodeGroupHelper.GetMenuTitle(id);
        }
    }
}