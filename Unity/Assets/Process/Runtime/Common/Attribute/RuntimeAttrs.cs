using System;

namespace Process.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomDataAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class IgnoreExportAttribute : Attribute
    {
        
    }
}