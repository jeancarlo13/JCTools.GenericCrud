using System;

namespace JCTools.GenericCrud.Settings
{
    public class CrudType
    {
        public Type Type
        {
            get;
            set;
        }
        public string KeyPropertyName
        {
            get;
            set;
        }

        public CrudType(Type type, string keyPropertyName = "Id")
        {
            Type = type;
            KeyPropertyName = keyPropertyName;
        }
    }
}