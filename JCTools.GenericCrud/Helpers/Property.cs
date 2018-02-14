using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JCTools.GenericCrud.Attibutes;

namespace JCTools.GenericCrud.Helpers
{
    internal class Property
    {
        public PropertyInfo Info { get; set; }
        public CrudListAttribute List { get; set; }
        public DisplayAttribute Display {get; set;}
    }
}