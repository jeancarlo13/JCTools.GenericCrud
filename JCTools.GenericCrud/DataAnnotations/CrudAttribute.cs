﻿using System;

namespace JCTools.GenericCrud.DataAnnotations
{
    /// <summary>
    /// Allows configured the property to be use into the CRUDs
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CrudAttribute: Attribute
    {
        /// <summary>
        /// True if the property is visible in the crud list; False another case
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// True if desired use a custom view; False another case.
        /// The custom views are find in the controller view folder with the name of the property preceded by "__Details" or "__Edit"
        /// </summary>
        public bool UseCustomView { get; set; }
    }
}