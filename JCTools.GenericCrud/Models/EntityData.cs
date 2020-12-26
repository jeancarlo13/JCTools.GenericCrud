using JCTools.GenericCrud.Settings;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;

namespace JCTools.GenericCrud.Models
{
    /// <summary>
    /// Represents the entire entity data for use into the CRUD views
    /// </summary>
    /// <typeparam name="TKey">The type of the property identifier of the entity model</typeparam>
    /// <typeparam name="TModel">The type of the model that represents the entities to modified</typeparam>    
    internal class EntityData<TModel, TKey> : IEntityData
        where TModel : class, new()
    {
        /// <summary>
        /// The real entity to be used
        /// </summary>
        private TModel _entity;

        /// <summary>
        /// The value of the Id/Key property of the entity
        /// </summary>
        private TKey _KeyValue;

        /// <summary>
        /// The data of all properties of the entity to be used in the CRUD views 
        /// </summary>
        public IEnumerable<PropertyData> Properties { get; }

        /// <summary>
        /// The data of the visible properties of the entity to be used in the CRUD views 
        /// </summary>
        public IEnumerable<PropertyData> VisibleProperties { get; }

        /// <summary>
        /// Initialize the instance
        /// </summary>
        /// <param name="entity">The entity to be use for the initialization</param>
        /// <param name="crudType">The type of the related crud to the entity to be used</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer"/> used for translate 
        /// the texts to displayed into the view</param>
        internal EntityData(TModel entity, ICrudType crudType, IStringLocalizer localizer)
        {
            _entity = entity;
            Properties = crudType.GetProperties(localizer, includeNoVisibleColumns: true);
            VisibleProperties = Properties.Where(p => p.IsVisible);
            _KeyValue = entity == null ? default(TKey) : (TKey)crudType.GetKeyPropertyValue(entity);
        }

        /// <summary>
        /// Provides access to the real entity 
        /// </summary>
        /// <returns>The real entity</returns>
        public object GetEntity() => _entity;

        /// <summary>
        /// Provides access to the Id/Key property value of the real entity 
        /// </summary>
        /// <returns>The Id/Key value</returns>
        public object GetKeyValue() => _KeyValue;

        /// <summary>
        /// Returns the property value of a specified object.
        /// </summary>
        /// <param name="property">The property to be review</param>
        /// <returns>The property value of the specified object.</returns>
        public object GetPropertyValue(PropertyData property)
            => property.GetValue(_entity);
    }
}