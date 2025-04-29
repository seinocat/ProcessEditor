using System;

namespace Process.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ProcessNodeAttribute : Attribute {}
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExportDataAttribute : Attribute {}
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DiscardNodeAttribute : Attribute {}
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServerJsonNodeAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ReferenceAttribute : Attribute
    {
        public Type type;
        public string fieldName;

        public ReferenceAttribute(Type type, string name)
        {
            this.type = type;
            fieldName = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ListReferenceAttribute : Attribute
    {
        public Type type;
        public string fieldName;

        public ListReferenceAttribute(Type type, string name)
        {
            this.type = type;
            fieldName = name;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class AssetReferenceAttribute : Attribute
    {
        public Type type;
        public string fieldName;

        public AssetReferenceAttribute(Type type, string name)
        {
            this.type = type;
            fieldName = name;
        }
        
    }
}