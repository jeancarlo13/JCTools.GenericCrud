using System;

namespace JCTools.GenericCrud.Attibutes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CrudListAttribute: Attribute
    {
        /// <summary>
        /// True if the property is visible in the crud list; False another case
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// The index of appearance into the crud list
        /// </summary>
        public int Order { get; set; }

    }
}