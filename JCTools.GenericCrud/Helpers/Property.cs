using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JCTools.GenericCrud.DataAnnotations;

namespace JCTools.GenericCrud.Helpers
{
    internal class Property
    {
        public PropertyInfo Info { get; set; }
        public CrudAttribute List { get; set; }
        public DisplayAttribute Display {get; set;}
    }
}