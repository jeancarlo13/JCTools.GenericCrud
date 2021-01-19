using System;

namespace JCTools.GenericCrud.Settings
{

    /// <summary>
    /// Represents the Id/Key property of a CRUD
    /// </summary>
    internal class KeyProperty : IKeyProperty
    {
        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of the property used how to key/id of the model
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// True indicates that the user can edit the value of the Id / Key property 
        /// and can overwrite its value; False (default) other case
        /// </summary>
        public bool IsEditable { get; set; }
    }
}