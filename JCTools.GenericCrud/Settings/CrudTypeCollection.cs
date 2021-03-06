using System;
using System.Collections.Generic;
using System.Linq;
using JCTools.GenericCrud.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Collection of the CRUDs types to be used
    /// </summary>
    public class CrudTypeCollection
    {
        /// <summary>
        /// The configured types collection
        /// </summary>
        private readonly List<ICrudType> _types;
        /// <summary>
        /// Initialize the default CRUD types collection
        /// </summary>
        public CrudTypeCollection()
            => _types = new List<ICrudType>();

        /// <summary>
        /// Allows add a new CRUD type
        /// </summary>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        /// <typeparam name="TModel">The type of the model to add</typeparam>
        public void Add<TModel>(string keyPropertyName = "Id")
            where TModel : class, new()
            => Add(new CrudType<TModel>(keyPropertyName));

        /// <summary>
        /// Allows add a new CRUD type using a custom controller 
        /// </summary>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        /// <typeparam name="TModel">The type of the model to add</typeparam>
        /// <typeparam name="TCustomController">The custom controller type to be used for the CRUD</typeparam>
        /// <typeparam name="TKey">The type of the property identifier of the entity model</typeparam>
        /// <typeparam name="TContext">The type of the database context to be used by get/stored the entities</typeparam>
        public void Add<TModel, TKey, TCustomController, TContext>(string keyPropertyName = "Id")
            where TModel : class, new()
            where TContext : DbContext
            where TCustomController : GenericController
            => Add(new CrudType<TModel, TKey, TCustomController, TContext>(keyPropertyName));

        /// <summary>
        /// Allows add a new CRUD type to the CRUD collection
        /// </summary>
        /// <param name="toAdd">The <see cref="ICrudType"/> instance to add</param>
        private void Add(ICrudType toAdd)
        {
            if (_types.Any(t => t.ModelType.Equals(toAdd.ModelType)))
                throw new ArgumentException($"A related CRUD has already been added to the model \"{toAdd.ModelType}\"");

            _types.Add(toAdd);
        }

        /// <summary>
        /// Returns a <see cref="List{ICrudType}"/> that iterates through the configured <see cref="ICrudType"/> items
        /// </summary>
        /// <param name="predicate">The filter to be applied to the crud enumeration</param>
        /// <returns>The generated list</returns>
        internal IReadOnlyList<ICrudType> ToList(Func<ICrudType, bool> predicate = null)
        {
            if (predicate == null)
                return _types;

            return _types.Where(predicate).ToList();
        }
        /// <summary>
        /// Allows get the CRUD with the specified model type
        /// </summary>
        /// <param name="modelName">The type of the related model to the searched CRUD</param>
        /// <value>The found CRUD or null</value>
        internal ICrudType this[string modelName]
        {
            get
            {
                var name = string.IsNullOrWhiteSpace(modelName)
                    ? throw new ArgumentNullException(nameof(modelName))
                    : modelName.ToLowerInvariant();
                return _types
                    .FirstOrDefault(c => c.ModelType.Name.ToLowerInvariant().Equals(name));
            }
        }

        /// <summary>
        /// Allows get the CRUD with the specified data
        /// </summary>
        /// <param name="model">The type of the related model to the searched CRUD</param>
        /// <param name="key">The name of the Key/Id property of the related model to the searched CRUD</param>
        /// <value>The found CRUD or null</value>
        internal ICrudType this[Type model, string key]
        {
            get => _types.FirstOrDefault(c => c.ModelType.Equals(model) && c.Key.Name == key);
        }

        /// <summary>
        /// Allows get the CRUD with the specified data
        /// </summary>
        /// <param name="routeData">The data of the current request that desire an CRUD controller</param>
        /// <value>The found CRUD or null</value>
        internal ICrudType this[RouteValueDictionary routeData]
        {
            get
            {
                if (routeData[Constants.ModelTypeTokenName] is Type modelType && modelType != null)
                {
                    var keyName = routeData[Constants.KeyTokenName]?.ToString() ?? "Id";
                    return this[modelType, keyName];
                }
                else if (routeData[Constants.EntitySettingsRouteKey] is string entityName
                            && !string.IsNullOrEmpty(entityName))
                {
                    return this[entityName];
                }

                return null;
            }
        }
    }
}

