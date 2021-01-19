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
        public CrudType(string keyPropertyName = "Id") : base(keyPropertyName) { }
        
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
                   .GetProperty(Key.Name)?
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
        /// The Id/Key property of the model used into the CRUD
        /// </summary>
        public IKeyProperty Key { get; set; }

        /// <summary>
        /// The controller to be used for entry to the CRUD actions
        /// </summary>
        /// <remarks>The default controller is <see cref="GenericController"/></remarks>
        public virtual Type ControllerType { get; }

        /// <summary>
        /// Gets or sets the mvc routes
        /// </summary>
        public IReadOnlyList<Route> Routes
        {
            get
            {
                if (_routes == null || !_routes.Any())
                {
                    var model = ModelType.Name;
                    _routes = new List<Route>()
                    {
                        new Route(this, Route.DetailsActionName, pattern: $"{model}/{{id}}/{Route.DetailsActionName}"),
                        new Route(this, Route.DeleteActionName, pattern: $"{model}/{{id}}/{Route.DeleteActionName}"),
                        new Route(this, Route.DeleteConfirmActionName, pattern: $"{model}/{{id}}/{Route.DeleteConfirmActionName}"),
                        new Route(this, Route.CreateActionName, pattern: $"{model}/{Route.CreateActionName}"),
                        new Route(this, Route.SaveActionName, pattern: $"{model}/{Route.SaveActionName}"),
                        new Route(this, Route.EditActionName, pattern: $"{model}/{{id}}/{Route.EditActionName}"),
                        new Route(this, Route.SaveChangesActionName, pattern: $"{model}/{{id}}/{Route.SaveChangesActionName}"),
                        new Route(this, Route.GetScriptActionName, pattern: $"{model}/{{filename}}.js"),
                        new Route(this, Route.IndexActionName,
                            routeName: string.Format(Route.RedirectIndexActionNamePattern, model),
                            pattern: $"{model}/{{id}}/{{message}}"
                        ),
                        new Route(this, Route.IndexActionName, pattern: model)
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
            var property = ModelType.GetProperty(keyPropertyName);
            Key = new KeyProperty()
            {
                Name = keyPropertyName,
                Type = property?.PropertyType
                    ?? throw new InvalidOperationException($"The \"{keyPropertyName}\" is not found in the model \"{ModelType.FullName}\""),
                IsEditable = property?.GetCustomAttribute<CrudAttribute>()?.IsEditableKey ?? false
            };

            ControllerType = typeof(GenericController);
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
                .GetProperty(Key.Name)?
                .GetValue(obj) ?? null;

            return null;
        }
    }
}