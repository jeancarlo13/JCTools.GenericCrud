using System;

namespace JCTools.GenericCrud.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CrudAttribute: Attribute
    {
        /// <summary>
        /// True if the property is visible in the crud list; False another case
        /// </summary>
        public bool Visible { get; set; } = true;

    }
}