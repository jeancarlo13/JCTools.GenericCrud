using System.Collections.Generic;

namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Defines the entire entity data for use into the CRUD views
    /// </summary>
    public interface IEntityData
    {
        /// <summary>
        /// The data of all properties of the entity to be used in the CRUD views 
        /// </summary>
        IEnumerable<PropertyData> Properties { get; }

        // <summary>
        /// The data of the visible properties of the entity to be used in the CRUD views 
        /// </summary>
        IEnumerable<PropertyData> VisibleProperties { get; }

        /// <summary>
        /// Provides access to the real entity 
        /// </summary>
        /// <returns>The real entity</returns>
        object GetEntity();

        /// <summary>
        /// Provides access to the Id/Key property value of the real entity 
        /// </summary>
        /// <returns>The Id/Key value</returns>
        object GetKeyValue();

        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="property">The property to be review</param>
        /// <returns>The property value of the specified object.</returns>
        object GetPropertyValue(PropertyData property);
    }
}