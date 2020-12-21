using System;
using System.Collections;
using System.Collections.Generic;
using JCTools.GenericCrud.Controllers;
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
        private readonly List<CrudType> _types;
        /// <summary>
        /// Initialize the default CRUD types collection
        /// </summary>
        public CrudTypeCollection()
            => _types = new List<CrudType>();

        /// <summary>
        /// Allows add a new CRUD type
        /// </summary>
        /// <param name="modelType">The type of the model to add</param>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        /// <param name="controllerName">The custom controller name to be used for the CRUD; string empty for used a generic controller</param>
        [Obsolete("Use other Add method", error: true)]
        public void Add(Type modelType, string keyPropertyName = "Id", string controllerName = "")
            => _types.Add(new CrudType(modelType, keyPropertyName, controllerName));

        /// <summary>
        /// Allows add a new CRUD type
        /// </summary>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        /// <typeparam name="TModel">The type of the model to add</typeparam>
        public void Add<TModel>(string keyPropertyName = "Id")
            where TModel : class, new()
            => _types.Add(new CrudType(typeof(TModel), keyPropertyName));

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
            where TCustomController : GenericController<TContext, TModel, TKey>
            => _types.Add(new CrudType(typeof(TModel), keyPropertyName, typeof(TCustomController).Name));

        /// <summary>
        /// Returns an enumerator that iterates through the configured <see cref="CrudType"/> items
        /// </summary>
        /// <returns>the generated enumerator</returns>
        internal IEnumerator<CrudType> GetEnumerator()
            => _types.GetEnumerator();
    }
}