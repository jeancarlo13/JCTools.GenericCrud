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

        public string Controller { get; set; }

        public CrudType(Type type, string keyPropertyName = "Id", string controller = "")
        {
            Type = type;
            KeyPropertyName = keyPropertyName;
            Controller = string.IsNullOrWhiteSpace(controller) ? string.Empty : controller;
        }
    }
}