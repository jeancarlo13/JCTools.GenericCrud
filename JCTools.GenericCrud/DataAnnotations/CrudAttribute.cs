﻿using System;

namespace JCTools.GenericCrud.DataAnnotations
{
    /// <summary>
    /// Allows configured the property to be use into the CRUDs
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CrudAttribute : Attribute
    {
        /// <summary>
        /// True if the property is visible in the crud list; False another case
        /// </summary>
        public bool Visible { get; set; } = true;
        /// <summary>
        /// True if desired use a custom view; False another case.
        /// </summary>
        /// <remarks>The custom views are find in the controller view folder with the name of the property 
        /// preceded by "_Details" or "_Edit"
        /// <para>eg;</para>
        /// <para>if the model property is named "Country" then the searched views will be:</para>
        /// <para>* _DetailsCountry.cshtml</para>
        /// <para>* _EditCountry.cshtml</para>
        /// </remarks>
        public bool UseCustomView { get; set; }

        /// <summary>
        /// True indicates that the related property is a property Id/Key 
        /// and the user can overwrite its value; False (default) another case
        /// </summary>
        /// <remarks>If the Id / Key property is editable, actually editing the entity 
        /// is a deletion of the old entity followed by an insertion of a new identity.</remarks>
        public bool IsEditableKey { get; set; }
    }
}