using System;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the Id/Key property of a CRUD
    /// </summary>
    public interface IKeyProperty
    {
        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of the property used how to key/id of the model
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// True indicates that the user can edit the value of the Id / Key property 
        /// and can overwrite its value; False (default) other case
        /// </summary>
        bool IsEditable { get; }
    }
}