using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JCTools.GenericCrud.Controllers;
using JCTools.GenericCrud.DataAnnotations;
using JCTools.GenericCrud.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace JCTools.GenericCrud.Settings
{
    /// <summary>
    /// Defines the properties required for generate a CRUD of any model
    /// </summary>
    /// <typeparam name="TModel">The type of the model to be used into the CRUD</typeparam>
    /// <typeparam name="TCustomController">The custom controller type to be used for the CRUD</typeparam>
    /// <typeparam name="TKey">The type of the property identifier of the entity model</typeparam>
    /// <typeparam name="TContext">The type of the database context to be used by get/stored the entities</typeparam>
    internal class CrudType<TModel, TKey, TCustomController, TContext> : CrudType<TModel>
        where TModel : class, new()
        where TContext : DbContext
        where TCustomController : GenericController

    {
        /// <summary>
        /// The controller to be used for entry to the CRUD actions
        /// </summary>
        /// <remarks>The default controller is <see cref="GenericController"/></remarks>
        public override Type ControllerType { get => typeof(TCustomController); }

        /// <summary>
        /// Generate a new instance for any model
        /// </summary>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        public CrudType(string keyPropertyName = "Id")
            : base(keyPropertyName)
        {
            UseGenericController = false;
        }

        /// <summary>
        /// Allows get the Key/Id property value of the specific instance
        /// </summary>
        /// <param name="obj">The instance to be evaluated</param>
        /// <returns>The found Key/Id property value or null</returns>
        public new TKey GetKeyPropertyValue(object obj)
        {
            if (obj is TModel instance)
            {
                var value = (TKey)ModelType
                   .GetTypeInfo()
                   .GetProperty(KeyPropertyName)?
                   .GetValue(obj);

                if (value != null)
                    return value;
            }

            return default(TKey);
        }
    }

    /// <summary>
    /// Defines the properties required for generate a CRUD of any model
    /// </summary>
    /// <typeparam name="TModel">The type of the model to be used into the CRUD</typeparam>
    internal class CrudType<TModel> : ICrudType, ICrudTypeRoutable
        where TModel : class, new()

    {
        /// <summary>
        /// Contains the mvc routes
        /// </summary>
        private IReadOnlyList<Route> _routes;

        /// <summary>
        /// The type of the model to be used into the CRUD
        /// </summary>
        public virtual Type ModelType { get => typeof(TModel); }

        /// <summary>
        /// The name of the property used how to key/id of the model
        /// </summary>
        public virtual string KeyPropertyName { get; }

        /// <summary>
        /// The type of the property used how to key/id of the model
        /// </summary>
        public Type KeyPropertyType { get; }

        /// <summary>
        /// True indicates that the user can edit the value of the Id / Key property 
        /// and can overwrite its value; False (default) other case
        /// </summary>
        public bool KeyPropertyIsEditable { get; }

        /// <summary>
        /// The controller to be used for entry to the CRUD actions
        /// </summary>
        /// <remarks>The default controller is <see cref="GenericController"/></remarks>
        public virtual Type ControllerType { get; }

        /// <summary>
        /// True if the controller to be used into the current represented CRUD 
        /// is <see cref="GenericController"/>; Another, false
        /// </summary>
        public bool UseGenericController { get; protected set; }

        /// <summary>
        /// The name of the model type related to the CRUD
        /// </summary>
        public string ModelTypeName => ModelType.Name;

        /// <summary>
        /// Gets or sets the mvc routes
        /// </summary>
        public IReadOnlyList<Route> Routes
        {
            get
            {
                if (_routes == null || !_routes.Any())
                {
                    _routes = new List<Route>()
                    {
                        new Route(this, Route.DetailsActionName, pattern: $"{ModelTypeName}/{{id}}/{Route.DetailsActionName}"),
                        new Route(this, Route.DeleteActionName, pattern: $"{ModelTypeName}/{{id}}/{Route.DeleteActionName}"),
                        new Route(this, Route.DeleteConfirmActionName, pattern: $"{ModelTypeName}/{{id}}/{Route.DeleteConfirmActionName}"),
                        new Route(this, Route.CreateActionName, pattern: $"{ModelTypeName}/{Route.CreateActionName}"),
                        new Route(this, Route.SaveActionName, pattern: $"{ModelTypeName}/{Route.SaveActionName}"),
                        new Route(this, Route.EditActionName, pattern: $"{ModelTypeName}/{{id}}/{Route.EditActionName}"),
                        new Route(this, Route.SaveChangesActionName, pattern: $"{ModelTypeName}/{{id}}/{Route.SaveChangesActionName}"),
                        new Route(this, Route.GetScriptActionName, pattern: $"{ModelTypeName}/{{filename}}.js"),
                        new Route(this, Route.IndexActionName,
                            routeName: string.Format(Route.RedirectIndexActionNamePattern, ModelTypeName),
                            pattern: $"{ModelTypeName}/{{id}}/{{message}}"
                        ),
                        new Route(this, Route.IndexActionName, pattern: ModelTypeName)
                    };
                }

                return _routes;
            }
        }

        /// <summary>
        /// Generate a new instance for any model
        /// </summary>
        /// <param name="keyPropertyName">The name of the property used how to key/id of the model</param>
        public CrudType(string keyPropertyName = "Id")
        {
            KeyPropertyName = keyPropertyName;
            var keyProperty = ModelType.GetProperty(KeyPropertyName);

            KeyPropertyType = keyProperty?.PropertyType
                ?? throw new InvalidOperationException($"The \"{KeyPropertyName}\" is not found in the model \"{ModelType.FullName}\"");

            KeyPropertyIsEditable = keyProperty.GetCustomAttribute<CrudAttribute>()?.IsEditableKey ?? false;

            ControllerType = typeof(GenericController);
            UseGenericController = true;
        }

        /// <summary>
        /// Allows get the properties that should appear into the CRUD views
        /// </summary>
        /// <param name="includeNoVisibleColumns">True for include the not visible columns; 
        /// False for return only the visible columns</param>
        /// <param name="localizer">The instance of <see cref="IStringLocalizer"/> used for translate 
        /// the texts to displayed into the view</param>
        /// <returns>The found properties</returns>
        public IEnumerable<PropertyData> GetProperties(IStringLocalizer localizer, bool includeNoVisibleColumns = false)
        {
            return ModelType.GetTypeInfo()
                .GetProperties()
                .Select(p => new PropertyData(p, localizer))
                .Where(p => p.IsVisible || includeNoVisibleColumns)
                .OrderBy(p => p.Order);
        }

        /// <summary>
        /// Allows get the Key/Id property value of the specific instance
        /// </summary>
        /// <param name="obj">The instance to be evaluated</param>
        /// <returns>The found Key/Id property value or null</returns>
        public object GetKeyPropertyValue(object obj)
        {
            if (obj is TModel instance)
                return ModelType.GetTypeInfo()
                .GetProperty(KeyPropertyName)?
                .GetValue(obj) ?? null;

            return null;
        }
    }
}